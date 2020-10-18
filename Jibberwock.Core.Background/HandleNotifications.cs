using Jibberwock.Core.Background.EmailBatchTypeHandlers;
using Jibberwock.DataModels.Core;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Cryptography;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Jibberwock.Core.Background
{
    public class HandleNotifications
    {
        private const int MaximumSendGridBatchSize = 1000;

        private readonly SendGridClient _sendGridClient;
        private readonly SqlServerDataSource _dataSource;
        private readonly WebApiConfiguration _webApiConfiguration;
        private readonly Dictionary<int, Func<ILogger, EmailBatch, EmailBatchTypeHandlerBase>> _emailBatchTypeHandlers;
        private readonly IHashCalculator _hashCalculator;

        public HandleNotifications(SqlServerDataSource dataSource, WebApiConfiguration webApiConfiguration, IHashCalculator hashCalculator)
        {
            _dataSource = dataSource;
            _webApiConfiguration = webApiConfiguration;
            _hashCalculator = hashCalculator;
            _sendGridClient = new SendGridClient(_webApiConfiguration.SendGrid.ApiKey);

            // The various types of email batches are pretty generic: notification, invitation, etc.
            // We have one TypeHandler per type, mapped in this dictionary.
            _emailBatchTypeHandlers = new Dictionary<int, Func<ILogger, EmailBatch, EmailBatchTypeHandlerBase>>()
                {
                    { 1, (log, eb) => new NotificationEmailBatchTypeHandler(log, _dataSource, eb, _webApiConfiguration.SendGrid) }
                };
        }

        [FunctionName("HandleNotifications")]
        public async Task Run(
            [ServiceBusTrigger("%CONFIGURATION_SERVICEBUS_QUEUES_NOTIFICATIONS%", Connection = "CONFIGURATION_SERVICEBUS_CONNECTIONSTRING")]
            Message notificationMessage,
            ILogger log, CancellationToken cancellationToken)
        {
            if (notificationMessage == null)
                throw new ArgumentNullException(nameof(notificationMessage));

            log.LogDebug($"HandleNotifications received message ID {notificationMessage.MessageId}. Getting details from the database...");

            cancellationToken.ThrowIfCancellationRequested();
            // Find the corresponding email batch. This might be null (email queued up, but then disabled/deleted in the database afterwards.)
            var getEmailBatchCommand = new Jibberwock.Persistence.DataAccess.Commands.Emails.GetBatch(log, notificationMessage.MessageId);
            var emailBatch = await getEmailBatchCommand.Execute(_dataSource);

            if (emailBatch == null)
            {
                log.LogInformation($"Message ID {notificationMessage.MessageId} was received, but it does not exist in the database or is marked as inactive. Ignoring.");
                return;
            }

            log.LogDebug($"Retrieved details from the database, proceeding to process message ID {notificationMessage.MessageId} (email batch {emailBatch.Id})");
            cancellationToken.ThrowIfCancellationRequested();

            var messageContents = Encoding.UTF8.GetString(notificationMessage.Body);
            dynamic parsedMessageContents = JsonSerializer.Deserialize<ExpandoObject>(messageContents);
            var messageMetadataObject = parsedMessageContents.Metadata;
                
            if (! _emailBatchTypeHandlers.TryGetValue(emailBatch.Type.Id, out var emailBatchTypeHandlerFactory)
                || emailBatchTypeHandlerFactory == null)
            {
                log.LogWarning($"Message ID {notificationMessage.MessageId} was received, but the corresponding email batch is of type {emailBatch.Type.Id}, which this function cannot handle. Ignoring.");
                return;
            }
            log.LogDebug($"Deserialised contents of message body for service bus message {notificationMessage.MessageId}.");
            cancellationToken.ThrowIfCancellationRequested();

            var emailBatchTypeHandler = emailBatchTypeHandlerFactory(log, emailBatch);
            cancellationToken.ThrowIfCancellationRequested();

            // Enable the devolved email batch to populate its details (subject, message, etc.)
            // Then, get its master list of personalisations.
            await emailBatchTypeHandler.PopulateAsync();
            log.LogDebug($"Successfully extracted details for email batch ID {emailBatch.Id}.");
            cancellationToken.ThrowIfCancellationRequested();

            var startBatchCommand = new Jibberwock.Persistence.DataAccess.Commands.Emails.StartBatch(log, emailBatch);
            IEnumerable<Personalization> personalisations = emailBatchTypeHandler.GetPersonalizations(messageMetadataObject);
            // Maximum number of 1000 personalisations per message, so group them up
            var personalisationBatches = from pers in
                                             personalisations.Select((pers, idx) => new { Index = idx, BatchId = idx / MaximumSendGridBatchSize, Personalisation = pers })
                                         group pers by pers.BatchId into batch
                                         orderby batch.Key
                                         select new { BatchId = batch.Key, Personalisations = batch.Select(b => b.Personalisation) };
            log.LogDebug($"Emails for email batch ID {emailBatch.Id} will be sent in {personalisationBatches.Count()} groups.");

            cancellationToken.ThrowIfCancellationRequested();
            // Start the batch record in the database, then run the SendGrid sends
            if (!(await startBatchCommand.Execute(_dataSource)))
            {
                log.LogWarning($"Message ID {notificationMessage.MessageId} is an email batch (ID: {emailBatch.Id}) which we can handle, but this function could not start the processing.");
                return;
            }

            long i = 0;

            foreach (var batch in personalisationBatches)
            {
                i++;

                var currentUtcDate = DateTime.UtcNow;
                var sgm = new SendGridMessage()
                {
                    Asm = new ASM() { GroupId = emailBatch.Type.UnsubscriptionGroupId },
                    TemplateId = emailBatch.Type.MessageTemplateId,
                    From = new EmailAddress(emailBatch.Type.SenderAddress, emailBatch.Type.SenderContact)
                };

                log.LogDebug($"Processing email group {i} for email batch ID {emailBatch.Id}.");

                try
                {
                    // Now that we know which personalisations we process, build up the list of emails to be sent.
                    // We need to know the "external email ID", then calculate the hashed To: address and its salt.
                    // The length of the salt matters here! It needs to be 16 bytes long in order to work, and we
                    // want to make sure that it's predictable in order to enable a basic "have you emailed me recently"
                    // feature to work at a later date.
                    var plannedEmailRecords = from p in batch.Personalisations
                                              let saltText = currentUtcDate.ToString("yyyy-MM..MM-yyyy")   // NB: the length of this text is important!
                                              let saltBytes = Encoding.UTF8.GetBytes(saltText)
                                              let hashedToAddress = _hashCalculator.CalculateHash(p.Tos[0].Email, saltBytes)
                                              select new Jibberwock.Persistence.DataAccess.TableTypes.Emails.Email(
                                                  p.CustomArgs[_webApiConfiguration.SendGrid.EmailIdParameterName],
                                                  saltBytes,
                                                  hashedToAddress
                                                  );
                    var emailRequest = new Jibberwock.Persistence.DataAccess.Commands.Emails.PrepareEmails(log, emailBatch, plannedEmailRecords.ToArray());
                    var createdEmailRecords = await emailRequest.Execute(_dataSource);

                    // Now, strip the list of personalisations of any excluded email addresses
                    sgm.Personalizations = (from p in batch.Personalisations
                                            let emailId = p.CustomArgs[_webApiConfiguration.SendGrid.EmailIdParameterName]
                                            where !createdEmailRecords.Item2.Contains(emailId)
                                            select p).ToList();
                    plannedEmailRecords = from per in plannedEmailRecords
                                          where !createdEmailRecords.Item2.Contains(per.ExternalEmailId)
                                          select per;
                    log.LogDebug($"Generated database records for email group {i} in email batch ID {emailBatch.Id}, will send {sgm.Personalizations.Count} emails.");

                    if (sgm.Personalizations.Any())
                    {
                        // No need to send an email if there aren't any personalisations!
                        await _sendGridClient.SendEmailAsync(sgm);
                        log.LogInformation($"Successfully sent email group {i} in email batch ID {emailBatch.Id} ({sgm.Personalizations.Count} emails).");

                        // Now, mark these emails as having been sent
                        var completeEmailsCommand = new Jibberwock.Persistence.DataAccess.Commands.Emails.CompleteEmails(log, emailBatch, currentUtcDate, plannedEmailRecords.ToArray());
                        var sentSuccessfully = await completeEmailsCommand.Execute(_dataSource);

                        // This is a very dangerous part of the execution - it could allow us to send the same email to people twice if we get it wrong
                        if (!sentSuccessfully)
                        {
                            log.LogError($"Emails were sent for email batch {emailBatch.Id}, but these emails could not be marked as sent in the database. If this batch throws an exception, they will be re-sent!");
                            foreach (var cer in createdEmailRecords.Item1)
                                log.LogError($"Email could not be marked as sent in the database. Internal ID: {cer.Id}, External ID: {cer.ExternalEmailId}.");
                        }
                    }
                    else
                    { log.LogDebug($"Will not try to send emails - there were no messages to be sent in email group {i} in email batch ID {emailBatch.Id}."); }

                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch(Exception ex)
                {
                    if (ex is OperationCanceledException)
                    { log.LogDebug($"Processing of email batch ID has been cancelled."); }
                    else
                    { log.LogError(ex, $"Exception occurred while processing group {i} of email batch ID {emailBatch.Id}. The state of emails may be inconsistent, and will be retried shortly."); }

                    throw;
                }
            }

            log.LogInformation($"All email groups for email batch ID {emailBatch.Id} have been processed.");

            emailBatch.ProcessedSuccessfully = true;
            var completeRequest = new Jibberwock.Persistence.DataAccess.Commands.Emails.CompleteBatch(log, emailBatch);
            var completed = await completeRequest.Execute(_dataSource);

            log.LogDebug($"Marked email batch ID as completed, leaving HandleNotifications.");
        }
    }
}

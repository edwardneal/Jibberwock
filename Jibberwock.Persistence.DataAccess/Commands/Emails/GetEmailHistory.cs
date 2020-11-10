using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.TableTypes.Emails;
using Jibberwock.Persistence.DataAccess.Utility;
using Jibberwock.Persistence.DataAccess.Utility.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Emails
{
    /// <summary>
    /// Gets all emails sent based upon specific criteria.
    /// </summary>
    public class GetEmailHistory : ValidatingCommand<IEnumerable<EmailBatch>, IReadableDataSource>
    {
        private IHashCalculator _hashCalculator;

        /// <summary>
        /// The start of the time window to retrieve.
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// The end of the time window to retrieve.
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// The email batch to retrieve emails from.
        /// </summary>
        public EmailBatch EmailBatch { get; set; }

        /// <summary>
        /// The type of the email batch(es) to retrieve emails from.
        /// </summary>
        public EmailBatchType EmailBatchType { get; set; }

        /// <summary>
        /// The Service Bus message ID of the email batch containing the emails to retrieve.
        /// </summary>
        public string ServiceBusMessageId { get; set; }

        /// <summary>
        /// The To: address of the emails to retreive.
        /// </summary>
        public string EmailAddress { get; set; }

        public GetEmailHistory(ILogger logger, IHashCalculator hashCalculator, DateTimeOffset? startTime, DateTimeOffset? endTime,
            EmailBatch emailBatch, EmailBatchType emailBatchType,
            string serviceBusMessageId, string emailAddress)
            : base(logger)
        {
            _hashCalculator = hashCalculator;

            StartTime = startTime;
            EndTime = endTime;
            EmailBatch = emailBatch;
            EmailBatchType = emailBatchType;
            ServiceBusMessageId = serviceBusMessageId;
            EmailAddress = emailAddress;
        }

        protected override async Task<IEnumerable<EmailBatch>> OnExecute(IReadableDataSource dataSource)
        {
            if (EmailBatch != null && EmailBatch.Id == 0)
                throw new ArgumentNullException(nameof(EmailBatch), "EmailBatch.Id must have a value.");
            if (EmailBatchType != null && EmailBatchType.Id == 0)
                throw new ArgumentNullException(nameof(EmailBatchType), "EmailBatchType.Id must have a value.");

            var databaseConnection = await dataSource.GetDbConnection();
            var toAddressHashes = new List<ToAddressHash>();

            // We don't know *exactly* what the possible hash is, so calculate a number of possibilities
            if (!string.IsNullOrWhiteSpace(EmailAddress))
            {
                // We need to have a start time for this
                if (!StartTime.HasValue)
                    throw new ArgumentNullException(nameof(StartTime), "If EmailAddress contains a value, StartTime must also contain a value.");

                var endTime = EndTime.HasValue ? EndTime.Value : DateTimeOffset.UtcNow;
                
                for (var currTime = StartTime.Value; currTime <= endTime; currTime = currTime.AddMonths(1))
                {
                    // Work out the month's salt, then calculate the email address hash
                    var saltBytes = Encoding.UTF8.GetBytes(currTime.ToString("yyyy-MM..MM-yyyy"));
                    var hashString = _hashCalculator.CalculateHash(EmailAddress.Trim().ToLower(), saltBytes);

                    toAddressHashes.Add(new ToAddressHash(saltBytes, hashString));
                }
            }

            var getHistoryReader = await databaseConnection.QueryMultipleAsync("core.usp_GetEmailHistory",
                new
                {
                    Start_Time = StartTime,
                    End_Time = EndTime,
                    Email_Batch_ID = EmailBatch?.Id,
                    Email_Batch_Type_ID = EmailBatchType?.Id,
                    External_Message_ID = ServiceBusMessageId,
                    To_Address_Hashes = toAddressHashes.AsTableValuedParameter("core.udt_ToAddressHash")
                },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            var dynBatchRecords = await getHistoryReader.ReadAsync();
            var emailBatches = (from eb in dynBatchRecords
                                select new EmailBatch()
                                {
                                    Id = eb.Id,
                                    ServiceBusMessageId = eb.ServiceBusMessageId,
                                    DateFirstProcessed = eb.DateFirstProcessed,
                                    DateLastProcessed = eb.DateLastProcessed,
                                    ProcessedSuccessfully = eb.ProcessedSuccessfully,
                                    Type = new EmailBatchType()
                                    {
                                        Id = eb.TypeId,
                                        Name = eb.TypeName
                                    }
                                }).ToArray();
            var ebDictionary = emailBatches.ToDictionary(eb => eb.Id);

            var dynEmailRecords = await getHistoryReader.ReadAsync();

            // Group the email records up into their corresponding batches
            foreach (var batch in emailBatches)
            {
                batch.Emails = (from eml in dynEmailRecords
                                where eml.EmailBatchId == batch.Id
                                select new Jibberwock.DataModels.Core.Email()
                                {
                                    Id = eml.Id,
                                    SendDate = eml.SendDate,
                                    ExternalEmailId = eml.ExternalEmailId
                                }).ToArray();
            }

            return emailBatches;
        }
    }
}

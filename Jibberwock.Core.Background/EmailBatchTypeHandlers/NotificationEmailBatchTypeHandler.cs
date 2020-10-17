using Jibberwock.DataModels.Core;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Core.Background.EmailBatchTypeHandlers
{
    public class NotificationEmailBatchTypeHandler : EmailBatchTypeHandlerBase
    {
        private Notification _notification;
        private IEnumerable<string> _emailAddresses;

        public NotificationEmailBatchTypeHandler(ILogger logger, IReadableDataSource dataSource, EmailBatch emailBatch, SendGridConfiguration sendGridConfiguration)
            : base(logger, dataSource, emailBatch, sendGridConfiguration)
        {
        }

        public override IEnumerable<Personalization> GetPersonalizations(dynamic messageMetadata)
        {
            foreach (var email in _emailAddresses)
            {
                var pers = GetPersonalization();

                pers.Subject = _notification.Subject;
                pers.SendAt = _notification.StartDate?.ToUnixTimeSeconds();
                pers.TemplateData = new
                {
                    Subject = _notification.Subject,
                    Message = _notification.Message,
                    Metadata = messageMetadata
                };

                pers.Tos = new List<EmailAddress>() { new EmailAddress(email) };

                yield return pers;
            }
        }

        public override async Task PopulateAsync()
        {
            var getNotificationEmailBatchCommand = new Jibberwock.Persistence.DataAccess.Commands.Emails.GetNotificationBatch(_logger, EmailBatch);
            var batchDetails = await getNotificationEmailBatchCommand.Execute(_dataSource);

            _notification = batchDetails.Item1;
            _emailAddresses = batchDetails.Item2;
        }
    }
}

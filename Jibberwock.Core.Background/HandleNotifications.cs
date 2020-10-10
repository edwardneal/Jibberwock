using Jibberwock.Persistence.DataAccess.DataSources;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Core.Background
{
    public class HandleNotifications
    {
        private readonly SqlServerDataSource _dataSource;

        public HandleNotifications(SqlServerDataSource dataSource) => _dataSource = dataSource;

        [FunctionName("HandleNotifications")]
        public async Task Run(
            [ServiceBusTrigger("%CONFIGURATION_SERVICEBUS_QUEUES_NOTIFICATIONS%", Connection = "CONFIGURATION_SERVICEBUS_CONNECTIONSTRING")]
            Message notificationMessage,
            ILogger log)
        {
            if (notificationMessage == null)
                throw new ArgumentNullException(nameof(notificationMessage));

            log.LogDebug($"HandleNotifications received message ID {notificationMessage.MessageId}. Getting details from the database...");

            var getEmailBatchCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.GetEmailBatch(log, notificationMessage.MessageId);
            var emailBatch = await getEmailBatchCommand.Execute(_dataSource);

            if (emailBatch == null)
            {
                log.LogInformation($"Message ID {notificationMessage.MessageId} was received, but it does not exist in the database or is marked as inactive. Ignoring.");
                return;
            }

            log.LogDebug($"Retrieved details from the database, proceeding to process message ID {notificationMessage.MessageId} (email batch {emailBatch.Id})");

            throw new NotImplementedException();
        }
    }
}

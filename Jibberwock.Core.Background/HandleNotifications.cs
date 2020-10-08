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
            log.LogInformation($"Received message on queue: {notificationMessage}");
        }
    }
}

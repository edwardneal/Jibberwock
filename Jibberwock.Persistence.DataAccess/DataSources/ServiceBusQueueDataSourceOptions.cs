using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// Configuration options which describe how Service Bus Queues should behave.
    /// </summary>
    public class ServiceBusQueueDataSourceOptions : IOptions<ServiceBusQueueDataSourceOptions>
    {
        /// <summary>
        /// The name of the Azure Service Bus namespace containing all relevant queues.
        /// </summary>
        public string NamespaceUrl { get; set; }

        /// <summary>
        /// Contains all queue names.
        /// </summary>
        public IEnumerable<string> QueueNames { get; set; }

        ServiceBusQueueDataSourceOptions IOptions<ServiceBusQueueDataSourceOptions>.Value => this;
    }
}

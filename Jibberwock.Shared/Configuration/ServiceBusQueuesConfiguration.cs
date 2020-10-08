using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration
{
    /// <summary>
    /// Contains the names of all Service Bus queues needed for specific purposes.
    /// </summary>
    public class ServiceBusQueuesConfiguration
    {
        /// <summary>
        /// The queue used to send/receive requests for email notifications.
        /// </summary>
        public string Notifications { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration
{
    /// <summary>
    /// Contains the Azure Service Bus configuration used to send and receive messages.
    /// </summary>
    public class ServiceBusConfiguration
    {
        /// <summary>
        /// The name of the Azure Service Bus namespace containing all relevant queues.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Contains all queue names.
        /// </summary>
        public ServiceBusQueuesConfiguration Queues { get; set; }
    }
}

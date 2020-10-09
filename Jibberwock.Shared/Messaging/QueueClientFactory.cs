using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Messaging
{
    /// <summary>
    /// Creates a <see cref="ServiceBusConnection"/>, authenticated using Azure AD (without a SAS token.)
    /// </summary>
    public static class QueueClientFactory
    {
        //private const string ServiceBusResource = "https://servicebus.azure.net/";

        /// <summary>
        /// Creates an authenticated <see cref="QueueClient"/>, authenticated using Azure AD.
        /// </summary>
        /// <param name="namespaceUrl">The fully-qualified Service Bus namespace.</param>
        /// <param name="queueName">The name of the Service Bus queue.</param>
        /// <returns>An authenticated <see cref="QueueClient"/>.</returns>
        public static QueueClient CreateQueueClient(string namespaceUrl, string queueName, RetryPolicy retryPolicy)
        {
            var tokenProvider = new AzureServiceTokenProvider();
            var queueClient = new QueueClient(namespaceUrl, queueName, new ManagedIdentityTokenProvider(tokenProvider), retryPolicy: retryPolicy);

            return queueClient;
        }
    }
}

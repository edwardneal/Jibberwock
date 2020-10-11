using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.DataSources
{
    /// <summary>
    /// This is an Azure Service Bus data source, capable of writing to an Azure Service Bus queue.
    /// </summary>
    public class ServiceBusQueueDataSource : IQueueDataSource
    {
        private readonly AzureServiceTokenProvider _tokenProvider;
        private readonly Dictionary<string, QueueClient> _queueClients;

        public object ConnectionRetryPolicy => throw new NotImplementedException();

        /// <summary>
        /// Configuration options which describe how this Azure Service Bus data source's connections should behave.
        /// </summary>
        public ServiceBusQueueDataSourceOptions DataSourceOptions { get; private set; }

        public ServiceBusQueueDataSource(IOptions<ServiceBusQueueDataSourceOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Value == null)
                throw new ArgumentNullException(nameof(options));

            DataSourceOptions = options.Value;

            _tokenProvider = new AzureServiceTokenProvider();
            _queueClients = (DataSourceOptions.QueueNames ?? Enumerable.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToDictionary(queueName => queueName.ToLower(),
                    queueName =>
                        new QueueClient(DataSourceOptions.NamespaceUrl, queueName.ToLower(), new ManagedIdentityTokenProvider(_tokenProvider), retryPolicy: RetryPolicy.Default));
        }

        internal QueueClient GetQueueClient(string queueName)
        {
            return _queueClients.TryGetValue(queueName.ToLower(), out var client) ? client : throw new IndexOutOfRangeException("Cannot find a queue with this name.");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            // Force every QueueClient to close
            await Task.WhenAll(from qn in _queueClients.Keys
                               where _queueClients[qn] != null
                                && !_queueClients[qn].IsClosedOrClosing
                               select _queueClients[qn].CloseAsync());
        }

        public Task OpenAsync()
        {
            // A Service Bus connection is implicitly opened when sending a message.
            return Task.CompletedTask;
        }
    }
}

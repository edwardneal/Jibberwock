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
    public abstract class EmailBatchTypeHandlerBase
    {
        protected readonly ILogger _logger;
        protected readonly IReadableDataSource _dataSource;
        protected readonly SendGridConfiguration _sendGridConfiguration;

        public EmailBatch EmailBatch { get; private set; }

        protected EmailBatchTypeHandlerBase(ILogger logger, IReadableDataSource dataSource, EmailBatch emailBatch, SendGridConfiguration sendGridConfiguration)
        {
            _logger = logger;
            _dataSource = dataSource;
            _sendGridConfiguration = sendGridConfiguration;

            EmailBatch = emailBatch;
        }

        public abstract Task PopulateAsync();

        public abstract IEnumerable<Personalization> GetPersonalizations(dynamic messageMetadata);

        protected Personalization GetPersonalization()
        {
            return new Personalization()
            {
                CustomArgs = new Dictionary<string, string>()
                    {
                        { _sendGridConfiguration.NotificationIdParameterName, EmailBatch.ServiceBusMessageId },
                        { _sendGridConfiguration.EmailIdParameterName, Guid.NewGuid().ToString() }
                    }
            };
        }
    }
}

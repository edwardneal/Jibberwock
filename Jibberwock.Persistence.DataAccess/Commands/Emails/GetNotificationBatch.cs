using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Emails
{
    /// <summary>
    /// Gets the details of an email batch of type Notification, based upon its email batch ID.
    /// </summary>
    public class GetNotificationBatch : ValidatingCommand<(Notification, IEnumerable<string>), IReadableDataSource>
    {
        /// <summary>
        /// The email batch to get the notification details of.
        /// </summary>
        [Required]
        public EmailBatch EmailBatch { get; set; }

        public GetNotificationBatch(ILogger logger, EmailBatch emailBatch)
            : base(logger)
        {
            EmailBatch = emailBatch;
        }

        protected override async Task<(Notification, IEnumerable<string>)> OnExecute(IReadableDataSource dataSource)
        {
            if (EmailBatch.Id == 0)
                throw new ArgumentNullException(nameof(EmailBatch), "EmailBatch.Id must have a value.");

            var databaseConnection = await dataSource.GetDbConnection();

            var notificationSet = await databaseConnection.QueryMultipleAsync("core.usp_GetEmailBatchByIdentifier",
                new { Email_Batch_ID = EmailBatch.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            var notification = await notificationSet.ReadFirstAsync<Notification>();
            var emailAddresses = await notificationSet.ReadAsync<string>();

            return (notification, emailAddresses);
        }
    }
}

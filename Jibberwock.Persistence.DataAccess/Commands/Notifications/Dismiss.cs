using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Security.Audit.EntryTypes;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.Commands.Auditing;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Notifications
{
    /// <summary>
    /// Dismisses a notification for a single user.
    /// </summary>
    public class Dismiss : AuditingCommand<bool, DismissNotification>
    {
        /// <summary>
        /// The notification to send (including target.)
        /// </summary>
        [Required]
        public Notification Notification { get; set; }

        public Dismiss(ILogger logger, User performedBy, string connectionId, int serviceId, string comment,
            Notification notification)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Notification = notification;
        }

        protected override async Task<bool> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, DismissNotification provisionalAuditTrailEntry)
        {
            if (Notification.Id == 0)
                throw new ArgumentNullException(nameof(Notification), "Notification.Id must have a value.");

            var databaseConnection = await dataSource.GetDbConnection();

            var success = await databaseConnection.ExecuteScalarAsync<bool>("core.usp_DismissNotification",
                new
                {
                    Calling_User_ID = PerformedBy.Id,
                    Notification_ID = Notification.Id
                }, commandType: CommandType.StoredProcedure, transaction: transaction, commandTimeout: 30);

            provisionalAuditTrailEntry.Notification = Notification;

            return success;
        }
    }
}

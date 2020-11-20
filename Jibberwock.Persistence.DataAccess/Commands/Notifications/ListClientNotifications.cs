using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Notifications
{
    /// <summary>
    /// List notifications for specific user (including global notifications, per-user notifications, per-tenant notifications.)
    /// </summary>
    public class ListClientNotifications : ValidatingCommand<IEnumerable<Notification>, IReadableDataSource>
    {
        /// <summary>
        /// The user which the notifications will be listed for.
        /// </summary>
        [Required]
        public User User { get; set; }

        public ListClientNotifications(ILogger logger, User user)
            : base(logger)
        {
            User = user;
        }

        protected override async Task<IEnumerable<Notification>> OnExecute(IReadableDataSource dataSource)
        {
            if (User != null && User.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(User), "User.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var results = await databaseConnection.QueryAsync<Notification, NotificationPriority, User, Tenant, Notification>(
                "core.usp_ListClientNotifications",
                (n, np, usr, ten) =>
                {
                    n.Priority = np;

                    if (usr != null && usr.Id != 0)
                    { n.TargetUser = usr; }

                    if (ten != null && ten.Id != 0)
                    { n.TargetTenant = ten; }

                    return n;
                }, new
                {
                    Calling_User_ID = User.Id,
                }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return results;
        }
    }
}

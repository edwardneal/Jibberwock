using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Notifications
{
    /// <summary>
    /// List notifications for a user, for a tenant or for all users.
    /// </summary>
    public class ListNotifications : ValidatingCommand<IEnumerable<Notification>, IReadableDataSource>
    {
        /// <summary>
        /// If present, filters the list of notifications to this user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// If present, filters the list of notifications to users associated with this tenant.
        /// </summary>
        public Tenant Tenant { get; set; }

        public ListNotifications(ILogger logger, User user)
            : base(logger)
        {
            User = user;
        }

        public ListNotifications(ILogger logger, Tenant tenant)
            : base(logger)
        {
            Tenant = tenant;
        }

        public ListNotifications(ILogger logger)
            : this(logger, user: null)
        { }

        protected override async Task<IEnumerable<Notification>> OnExecute(IReadableDataSource dataSource)
        {
            if (User != null && User.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(User), "User.Id must have a value");
            if (Tenant != null && Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.Id must have a value");
            if (User != null && User.Id != 0 && Tenant != null && Tenant.Id != 0)
                throw new ArgumentException("User.Id and Tenant.Id cannot both have values");

            var databaseConnection = await dataSource.GetDbConnection();

            var results = await databaseConnection.QueryAsync<Notification, NotificationPriority, User, Tenant, EmailBatch, Notification>(
                "core.usp_ListNotifications",
                (n, np, usr, ten, eb) =>
                {
                    n.Priority = np;

                    if (usr != null && usr.Id != 0)
                    { n.TargetUser = usr; }

                    if (ten != null && ten.Id != 0)
                    { n.TargetTenant = ten; }

                    if (eb != null && eb.Id != 0)
                    { n.EmailBatch = eb; }
                    return n;
                }, new
                {
                    User_ID = User?.Id,
                    Tenant_ID = Tenant?.Id
                }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return results;
        }
    }
}

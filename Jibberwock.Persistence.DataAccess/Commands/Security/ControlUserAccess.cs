using Dapper;
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

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Enables or disables a user.
    /// </summary>
    public class ControlUserAccess : AuditingCommand<bool, Jibberwock.DataModels.Security.Audit.EntryTypes.ControlUserAccess>
    {
        /// <summary>
        /// The desired state of the user.
        /// </summary>
        [Required]
        public User User { get; set; }

        public ControlUserAccess(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, User user)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            User = user;
        }

        protected override async Task<bool> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.ControlUserAccess provisionalAuditTrailEntry)
        {
            if (User.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(User), "User.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var successfullyControlled = await databaseConnection.ExecuteScalarAsync<int>("security.usp_ControlUserAccess",
                new { User_ID = User.Id, Enabled = User.Enabled },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            provisionalAuditTrailEntry.RelatedUser = User;
            provisionalAuditTrailEntry.Enabled = User.Enabled;

            return successfullyControlled == 0;
        }
    }
}

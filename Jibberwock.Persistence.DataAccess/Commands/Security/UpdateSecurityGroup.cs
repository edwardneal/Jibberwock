using Dapper;
using Jibberwock.DataModels.Security;
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

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Updates a single group.
    /// </summary>
    public class UpdateSecurityGroup : AuditingCommand<Group, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyGroup>
    {
        /// <summary>
        /// The desired state of the group.
        /// </summary>
        [Required]
        public Group Group { get; set; }

        public UpdateSecurityGroup(ILogger logger, User performedBy, string connectionId, long serviceId, string comment, Group group)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Group = group;
        }

        protected override async Task<Group> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyGroup provisionalAuditTrailEntry)
        {
            if (Group.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Group), "Group.Id must have a value");
            if (string.IsNullOrWhiteSpace(Group.Name))
                throw new ArgumentOutOfRangeException(nameof(Group), "Group.Name must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            provisionalAuditTrailEntry.RelatedTenant = Group.Tenant;

            var resultantGroup = await databaseConnection.QuerySingleAsync<Group>("security.usp_UpdateSecurityGroup",
                new { Security_Group_ID = Group.Id, Name = Group.Name },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            Group = resultantGroup;

            provisionalAuditTrailEntry.Group = resultantGroup;
            provisionalAuditTrailEntry.NewGroup = false;

            return resultantGroup;
        }
    }
}

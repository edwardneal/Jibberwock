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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Removes a group.
    /// </summary>
    public class DeleteSecurityGroup : AuditingCommand<bool, Jibberwock.DataModels.Security.Audit.EntryTypes.DeleteGroup>
    {
        /// <summary>
        /// The group to remove.
        /// </summary>
        [Required]
        public Group Group { get; set; }

        public DeleteSecurityGroup(ILogger logger, User performedBy, string connectionId, long serviceId, string comment, Group group)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Group = group;
        }

        protected override async Task<bool> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.DeleteGroup provisionalAuditTrailEntry)
        {
            if (Group.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Group), "Group.Id must have a value");
            if (Group.Tenant == null)
                throw new ArgumentOutOfRangeException(nameof(Group), "Group.Tenant must have a value");
            if (Group.Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Group), "Group.Tenant.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            provisionalAuditTrailEntry.RelatedTenant = Group.Tenant;

            var getGroupCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetSecurityGroupById(Logger, Group);
            var originalGroup = await getGroupCommand.Execute(dataSource.GetReadableDataSource());

            var successfullyDeleted = await databaseConnection.ExecuteScalarAsync<bool>("security.usp_DeleteSecurityGroup",
                new { Tenant_ID = Group.Tenant.Id, Security_Group_ID = Group.Id },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            provisionalAuditTrailEntry.Group = originalGroup;

            return successfullyDeleted;
        }
    }
}

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
    /// Creates a single group.
    /// </summary>
    public class CreateSecurityGroup : AuditingCommand<Group, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyGroup>
    {
        /// <summary>
        /// The desired state of the group.
        /// </summary>
        [Required]
        public Group Group { get; set; }

        public CreateSecurityGroup(ILogger logger, User performedBy, string connectionId, long serviceId, string comment, Group group)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Group = group;
        }

        protected override async Task<Group> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyGroup provisionalAuditTrailEntry)
        {
            if (Group.Id != 0)
                throw new ArgumentOutOfRangeException(nameof(Group), "Group.Id must not have a value");
            if (string.IsNullOrWhiteSpace(Group.Name))
                throw new ArgumentOutOfRangeException(nameof(Group), "Group.Name must have a value");

            var databaseConnection = await dataSource.GetDbConnection();
            var creationParameters = new DynamicParameters(new
            {
                Name = Group.Name,
                Tenant_ID = Group.Tenant?.Id
            });

            creationParameters.Add("Security_Group_ID", dbType: DbType.Int64, direction: ParameterDirection.Output);

            provisionalAuditTrailEntry.RelatedTenant = Group.Tenant;

            // Create the group itself, then iterate through its ACLs and memberships, creating those
            await databaseConnection.ExecuteAsync("security.usp_CreateSecurityGroup", creationParameters,
                commandType: CommandType.StoredProcedure, transaction: transaction, commandTimeout: 30);

            Group.Id = creationParameters.Get<long>("Security_Group_ID");

            provisionalAuditTrailEntry.Group = Group;
            provisionalAuditTrailEntry.NewGroup = true;

            return Group;
        }

        protected override async Task OnCommandCompleted(IReadWriteDataSource dataSource, ModifyGroup auditTrailEntry, Group result)
        {
            if (Group.Users != null)
            {
                foreach (var mem in Group.Users)
                {
                    var createMembershipCommand = new CreateSecurityGroupMembership(Logger, PerformedBy, ConnectionId, OriginatingService.Id, Comment,
                        new GroupMembership() { Group = Group, User = mem.User, Enabled = mem.Enabled });

                    await createMembershipCommand.Execute(dataSource);
                }
            }

            if (Group.AccessControlEntries != null)
            {
                foreach (var ace in Group.AccessControlEntries)
                {
                    var createACECommand = new CreateAccessControlEntry(Logger, PerformedBy, ConnectionId, OriginatingService.Id, Comment,
                        new AccessControlEntry() { Group = Group, Permission = ace.Permission, Resource = ace.Resource });

                    await createACECommand.Execute(dataSource);
                }
            }
        }
    }
}

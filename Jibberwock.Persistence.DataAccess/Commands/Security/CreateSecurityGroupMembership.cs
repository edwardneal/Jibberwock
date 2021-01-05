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
    /// Creates a membership in a single group.
    /// </summary>
    public class CreateSecurityGroupMembership : AuditingCommand<GroupMembership, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyGroupMembership>
    {
        /// <summary>
        /// The desired group membership.
        /// </summary>
        [Required]
        public GroupMembership GroupMembership { get; set; }

        public CreateSecurityGroupMembership(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, GroupMembership groupMembership)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            GroupMembership = groupMembership;
        }

        protected override async Task<GroupMembership> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyGroupMembership provisionalAuditTrailEntry)
        {
            if (GroupMembership.Group == null)
                throw new ArgumentOutOfRangeException(nameof(GroupMembership), "GroupMembership.Group must have a value");
            if (GroupMembership.Group.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(GroupMembership), "GroupMembership.Group.Id must have a value");
            if (GroupMembership.Group.Tenant == null)
                throw new ArgumentOutOfRangeException(nameof(GroupMembership), "GroupMembership.Group.Tenant must have a value");
            if (GroupMembership.Group.Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(GroupMembership), "GroupMembership.Group.Tenant.Id must have a value");
            if (GroupMembership.User == null)
                throw new ArgumentOutOfRangeException(nameof(GroupMembership), "GroupMembership.User must have a value");
            if (GroupMembership.User.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(GroupMembership), "GroupMembership.User.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            provisionalAuditTrailEntry.RelatedTenant = GroupMembership.Group.Tenant;
            provisionalAuditTrailEntry.RelatedUser = GroupMembership.User;

            var resultantMemberships = await databaseConnection.QueryAsync<GroupMembership, Group, User, GroupMembership>("security.usp_CreateSecurityGroupMembership",
                (sgm, grp, usr) =>
                {
                    if (sgm != null && grp != null)
                    { sgm.Group = grp; }

                    if (sgm != null && usr != null)
                    { sgm.User = usr; }

                    return sgm;
                },
                new { Tenant_ID = GroupMembership.Group.Tenant.Id, Security_Group_ID = GroupMembership.Group.Id, Member_User_ID = GroupMembership.User.Id, Enabled = GroupMembership.Enabled },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            GroupMembership = resultantMemberships.FirstOrDefault();

            provisionalAuditTrailEntry.GroupMembership = GroupMembership;
            provisionalAuditTrailEntry.NewGroupMembership = false;

            return GroupMembership;
        }
    }
}

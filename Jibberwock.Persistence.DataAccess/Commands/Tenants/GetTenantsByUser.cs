using Dapper;
using Jibberwock.DataModels.Security;
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

namespace Jibberwock.Persistence.DataAccess.Commands.Tenants
{
    /// <summary>
    /// Gets all tenants which the given <see cref="User"/> is a member of.
    /// </summary>
    public class GetTenantsByUser : ValidatingCommand<IEnumerable<Tenant>, IReadableDataSource>
    {
        /// <summary>
        /// The user to get the tenants of.
        /// </summary>
        [Required]
        public User User { get; set; }

        /// <summary>
        /// If <c>true</c>, this will only account for currently-active group memberships.
        /// </summary>
        public bool ActiveMembershipsOnly { get; set; }

        public GetTenantsByUser(ILogger logger, User user, bool activeMembershipsOnly)
            : base(logger)
        {
            User = user;
            ActiveMembershipsOnly = activeMembershipsOnly;
        }

        protected override async Task<IEnumerable<Tenant>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var userTenants = await databaseConnection.QueryAsync<Tenant, Group, GroupMembership, Tenant>("tenants.usp_GetTenantsByUserId",
                (ten, grp, grpMem) =>
                {
                    // Tie the group to the WellKnownGroups dictionary if we can...
                    if (ten != null && grp != null)
                    {
                        ten.WellKnownGroups = new Dictionary<WellKnownGroupType, Group>()
                            { { WellKnownGroupType.TenantMembers, grp } };
                    }
                    // ...then the group membership to the group...
                    if (grp != null && grpMem != null)
                    {
                        grp.Users = new[] { grpMem };
                    }
                    // ...then the user to the group membership
                    if (grpMem != null)
                    {
                        grpMem.User = User;
                    }

                    return ten;
                },
                new { User_ID = User.Id, Active_Memberships_Only = ActiveMembershipsOnly },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return userTenants;
        }
    }
}

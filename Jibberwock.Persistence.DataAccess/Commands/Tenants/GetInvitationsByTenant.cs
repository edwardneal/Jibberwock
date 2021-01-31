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
    /// Gets all invitations to the specified <see cref="DataModels.Tenants.Tenant"/>, optionally including inapplicable invitation records.
    /// </summary>
    public class GetInvitationsByTenant : ValidatingCommand<IEnumerable<Invitation>, IReadableDataSource>
    {
        /// <summary>
        /// The tenant to get the invitations to.
        /// </summary>
        [Required]
        public Tenant Tenant { get; set; }

        /// <summary>
        /// If <c>true</c>, this will include all invitations which have expired or been revoked.
        /// </summary>
        public bool IncludeInapplicableInvitations { get; set; }

        public GetInvitationsByTenant(ILogger logger, Tenant tenant, bool includeInapplicableInvitations)
            : base(logger)
        {
            Tenant = tenant;
            IncludeInapplicableInvitations = includeInapplicableInvitations;
        }

        protected override async Task<IEnumerable<Invitation>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var tenantInvitations = await databaseConnection.QueryAsync<Invitation, User, Invitation>("tenants.usp_GetInvitationsByTenant",
                (inv, usr) =>
                {
                    if (inv != null && usr != null && usr.Id != 0)
                    {
                        inv.InvitedUser = usr;
                    }

                    return inv;
                },
                new { Tenant_ID = Tenant.Id, Include_Inapplicable = IncludeInapplicableInvitations },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return tenantInvitations;
        }
    }
}

using Dapper;
using Jibberwock.DataModels.Security;
using Jibberwock.DataModels.Security.Audit.EntryTypes;
using Jibberwock.DataModels.Tenants;
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

namespace Jibberwock.Persistence.DataAccess.Commands.Tenants
{
    /// <summary>
    /// Revokes an invitation.
    /// </summary>
    public class RevokeInvitation : AuditingCommand<bool, Jibberwock.DataModels.Security.Audit.EntryTypes.RevokeInvitation>
    {
        /// <summary>
        /// The invitation to revoke.
        /// </summary>
        [Required]
        public Invitation Invitation { get; set; }

        public RevokeInvitation(ILogger logger, User performedBy, string connectionId, long serviceId, string comment, Invitation invitation)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Invitation = invitation;
        }

        protected override async Task<bool> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.RevokeInvitation provisionalAuditTrailEntry)
        {
            if (Invitation.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Invitation), "Invitation.Id must have a value");
            if (Invitation.Tenant == null)
                throw new ArgumentOutOfRangeException(nameof(Invitation), "Invitation.Tenant must have a value");
            if (Invitation.Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Invitation), "Invitation.Tenant.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            provisionalAuditTrailEntry.RelatedTenant = Invitation.Tenant;

            var successfullyDeleted = await databaseConnection.ExecuteScalarAsync<bool>("tenants.usp_RevokeInvitation",
                new { Tenant_ID = Invitation.Tenant.Id, Invitation_ID = Invitation.Id },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            provisionalAuditTrailEntry.Invitation = Invitation;

            return successfullyDeleted;
        }
    }
}

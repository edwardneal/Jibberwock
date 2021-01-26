using Dapper;
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
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Tenants
{
    /// <summary>
    /// Synchronises a tenant's details from a billing provider.
    /// </summary>
    public class SyncTenantFromBillingProvider : AuditingCommand<Tenant, ModifyTenant>
    {
        /// <summary>
        /// The required end state of the tenant to be synchronised.
        /// </summary>
        [Required]
        public Tenant Tenant { get; set; }

        public SyncTenantFromBillingProvider(ILogger logger, User performedBy, string connectionId, long serviceId, string comment,
            Tenant tenant)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Tenant = tenant;
        }

        protected override async Task<Tenant> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyTenant provisionalAuditTrailEntry)
        {
            if (string.IsNullOrWhiteSpace(Tenant.ExternalId))
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.ExternalId must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            await databaseConnection.ExecuteAsync("tenants.usp_SyncTenantFromBillingProvider",
                new
                {
                    Tenant_ID = Tenant.Id == 0 ? (long?)null : Tenant.Id,
                    Tenant_Name = Tenant.Name,
                    Tenant_External_Identifier = Tenant.ExternalId,
                    Contact_Telephone_Number = Tenant.BillingContact.TelephoneNumber,
                    Contact_Email_Address = Tenant.BillingContact.EmailAddress
                }, transaction: transaction, commandType: CommandType.StoredProcedure, commandTimeout: 30);

            provisionalAuditTrailEntry.NewTenant = false;
            provisionalAuditTrailEntry.Tenant = Tenant;
            provisionalAuditTrailEntry.RelatedTenant = Tenant;

            return Tenant;
        }
    }
}

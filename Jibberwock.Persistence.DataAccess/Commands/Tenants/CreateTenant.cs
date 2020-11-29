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
    /// Creates a tenant.
    /// </summary>
    public class CreateTenant : AuditingCommand<Tenant, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyTenant>
    {
        /// <summary>
        /// The tenant to be created.
        /// </summary>
        [Required]
        public Tenant Tenant { get; set; }

        public CreateTenant(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, Tenant tenant)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Tenant = tenant;
        }

        protected override async Task<Tenant> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyTenant provisionalAuditTrailEntry)
        {
            if (Tenant.Id != 0)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.Id must not have a value");
            if (string.IsNullOrWhiteSpace(Tenant.Name))
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.Name must have a value");
            if (Tenant.BillingContact == null)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.BillingContact must have a value");
            if (Tenant.BillingContact.Id != 0)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.BillingContact.Id must not have a value");
            if (string.IsNullOrWhiteSpace(Tenant.BillingContact.FullName))
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.BillingContact.FullName must have a value");
            if (string.IsNullOrWhiteSpace(Tenant.BillingContact.EmailAddress) && string.IsNullOrWhiteSpace(Tenant.BillingContact.TelephoneNumber))
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.BillingContact.EmailAddress or Tenant.BillingContact.TelephoneNumber must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var tenantId = await databaseConnection.ExecuteScalarAsync<long>("tenants.usp_CreateTenant",
                new
                {
                    Name = Tenant.Name,
                    Contact_Name = Tenant.BillingContact.FullName,
                    Contact_Telephone_Number = Tenant.BillingContact.TelephoneNumber,
                    Contact_Email_Address = Tenant.BillingContact.EmailAddress,
                    Current_User_ID = PerformedBy.Id
                }, transaction: transaction, commandType: CommandType.StoredProcedure, commandTimeout: 30);

            Tenant.Id = tenantId;

            provisionalAuditTrailEntry.NewTenant = true;
            provisionalAuditTrailEntry.Tenant = Tenant;
            provisionalAuditTrailEntry.RelatedTenant = Tenant;

            return Tenant;
        }
    }
}

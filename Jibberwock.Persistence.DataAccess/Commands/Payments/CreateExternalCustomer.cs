using Dapper;
using Jibberwock.DataModels.Tenants;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.DataSources.Payments;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Payments
{
    /// <summary>
    /// Creates an external customer.
    /// </summary>
    public class CreateExternalCustomer : ValidatingCommand<Tenant, IReadWriteDataSource>
    {
        private readonly ICustomerDataSource _customerDataSource;

        /// <summary>
        /// The tenant to create.
        /// </summary>
        [Required]
        public Tenant Tenant { get; set; }

        public CreateExternalCustomer(ILogger logger, Tenant tenant, ICustomerDataSource customerDataSource)
            : base(logger)
        {
            _customerDataSource = customerDataSource;

            Tenant = tenant;
        }

        protected override async Task<Tenant> OnExecute(IReadWriteDataSource dataSource)
        {
            if (Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.Id must have a value");
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

            // NB: this technically means that a tenant's Created web hook could trigger before the record has been inserted into the database.
            // This is unlikely though - and there are no negative consequences to this race condition.
            Tenant.ExternalId = await _customerDataSource.CreateCustomer(Tenant.Name, Tenant.BillingContact.EmailAddress, Tenant.BillingContact.TelephoneNumber,
                new Dictionary<string, string>() { { "jibberwock_id", Tenant.Id.ToString() } });

            await databaseConnection.ExecuteAsync("tenants.usp_SyncTenantExternalIdentifier",
                new { Tenant_ID = Tenant.Id, External_Identifier = Tenant.ExternalId },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return Tenant;
        }
    }
}

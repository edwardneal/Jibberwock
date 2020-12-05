using Dapper;
using Jibberwock.DataModels.Tenants;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Tenants
{
    /// <summary>
    /// Gets the details of a specific tenant.
    /// </summary>
    public class GetTenantById : ValidatingCommand<Tenant, IReadableDataSource>
    {
        /// <summary>
        /// The ID of the tenant to query for.
        /// </summary>
        public long TenantId { get; set; }

        public GetTenantById(ILogger logger, long id)
            : base(logger)
        {
            TenantId = id;
        }

        protected override async Task<Tenant> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var tenantDetailsList = await databaseConnection.QueryAsync<Tenant, Contact, Tenant>("tenants.usp_GetTenantById",
                (ten, cont) =>
                {
                    if (cont != null && cont.Id != 0)
                        ten.BillingContact = cont;

                    return ten;
                },
                new { Tenant_ID = TenantId },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return tenantDetailsList.FirstOrDefault();
        }
    }
}

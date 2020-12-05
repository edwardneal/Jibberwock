using Dapper;
using Jibberwock.DataModels.Security;
using Jibberwock.DataModels.Tenants;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Lists all <see cref="Group"/>s in the named <see cref="Tenant"/>.
    /// </summary>
    public class ListTenantGroups : ValidatingCommand<IEnumerable<Group>, IReadableDataSource>
    {
        /// <summary>
        /// The <see cref="Tenant"/> to list the <see cref="Group"/>s of.
        /// </summary>
        [Required]
        public Tenant Tenant { get; set; }

        public ListTenantGroups(ILogger logger, Tenant tenant)
            : base(logger)
        {
            Tenant = tenant;
        }

        protected override async Task<IEnumerable<Group>> OnExecute(IReadableDataSource dataSource)
        {
            if (Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var tenantGroups = await databaseConnection.QueryAsync<Group>("security.usp_GetTenantSecurityGroups",
                new { Tenant_ID = Tenant.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return tenantGroups;
        }
    }
}

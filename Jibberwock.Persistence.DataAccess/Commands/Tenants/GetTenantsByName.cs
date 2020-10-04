using Dapper;
using Jibberwock.DataModels.Tenants;
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
    /// Gets all tenants which match a specific name.
    /// </summary>
    public class GetTenantsByName : ValidatingCommand<IEnumerable<Tenant>, IReadableDataSource>
    {
        /// <summary>
        /// The filter to apply to <see cref="Tenant.Name"/> when querying.
        /// </summary>
        [StringLength(256, ErrorMessage = "Name filter must be less than 256 characters long.", MinimumLength = 1)]
        public string NameFilter { get; set; }

        public GetTenantsByName(ILogger logger, string nameFilter)
            : base(logger)
        {
            NameFilter = nameFilter;
        }

        protected override async Task<IEnumerable<Tenant>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();
            var resultantFilter = NameFilter.Replace("*", "%");

            var matchingTenants = await databaseConnection.QueryAsync<Tenant>("tenants.usp_GetTenantsByName",
                new { Name_Filter = resultantFilter },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return matchingTenants;
        }
    }
}

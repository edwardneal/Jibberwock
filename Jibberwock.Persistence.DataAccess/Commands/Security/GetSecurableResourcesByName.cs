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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Gets all securable resources which match a specific name.
    /// </summary>
    public class GetSecurableResourcesByName : ValidatingCommand<IEnumerable<SecurableResource>, IReadableDataSource>
    {
        /// <summary>
        /// The filter to apply to the securable resource name when querying.
        /// </summary>
        [StringLength(256, ErrorMessage = "Name filter must be less than 256 characters long.", MinimumLength = 1)]
        public string NameFilter { get; set; }

        /// <summary>
        /// The user which is executing this command.
        /// </summary>
        [Required]
        public User CurrentUser { get; set; }

        /// <summary>
        /// The tenant to search in.
        /// </summary>
        public Tenant Tenant { get; set; }

        public GetSecurableResourcesByName(ILogger logger, string nameFilter, User currentUser, Tenant tenant)
            : base(logger)
        {
            NameFilter = nameFilter;
            CurrentUser = currentUser;
            Tenant = tenant;
        }

        protected override async Task<IEnumerable<SecurableResource>> OnExecute(IReadableDataSource dataSource)
        {
            if (CurrentUser.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(CurrentUser), "CurrentUser.Id must have a value");
            if (Tenant != null && Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();
            var resultantFilter = NameFilter.Replace("*", "%");

            var matchingObjects = await databaseConnection.QueryAsync("security.usp_GetSecurableResourcesByName",
                new { Name_Filter = resultantFilter, User_ID = CurrentUser.Id, Tenant_ID = Tenant?.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return (from sr in matchingObjects
                    select SecurableResourceHelpers.GetSecurableResourceFromDatabase(sr) as SecurableResource)
                    .ToArray();
        }
    }
}

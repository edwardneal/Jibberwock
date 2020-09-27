using Dapper;
using Jibberwock.DataModels.Users;
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
    /// Gets all users which match a specific name.
    /// </summary>
    public class GetUsersByName : ValidatingCommand<IEnumerable<User>, IReadableDataSource>
    {
        /// <summary>
        /// The filter to apply to <see cref="User.Name"/> when querying.
        /// </summary>
        [StringLength(128, ErrorMessage = "Name filter must be less than 128 characters long.", MinimumLength = 1)]
        public string NameFilter { get; set; }

        public GetUsersByName(ILogger logger, string nameFilter)
            : base(logger)
        {
            NameFilter = nameFilter;
        }

        protected override async Task<IEnumerable<User>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();
            var resultantFilter = NameFilter.Replace("*", "%");

            var matchingUsers = await databaseConnection.QueryAsync<User>("security.usp_GetUsersByName",
                new { Name_Filter = resultantFilter },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return matchingUsers;
        }
    }
}

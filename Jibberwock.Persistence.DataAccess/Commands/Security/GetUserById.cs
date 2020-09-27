using Dapper;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Gets the details and external identities of a specific user.
    /// </summary>
    public class GetUserById : ValidatingCommand<User, IReadableDataSource>
    {
        /// <summary>
        /// The ID of the user to query for.
        /// </summary>
        public long UserId { get; set; }

        public GetUserById(ILogger logger, long id)
            : base(logger)
        {
            UserId = id;
        }

        protected override async Task<User> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var userDetailsBatch = await databaseConnection.QueryMultipleAsync("security.usp_GetUserById",
                new { User_ID = UserId },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            var userDetails = await userDetailsBatch.ReadSingleOrDefaultAsync<User>();
            var externalIdentities = await userDetailsBatch.ReadAsync<ExternalIdentity>();

            userDetails.ExternalIdentities = externalIdentities;
            return userDetails;
        }
    }
}

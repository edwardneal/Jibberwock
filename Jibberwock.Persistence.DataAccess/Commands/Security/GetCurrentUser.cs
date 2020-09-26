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
    /// Checks whether or not the current user has all of the specified permissions over the specified securable resources.
    /// </summary>
    public class GetCurrentUser : ValidatingCommand<User, IReadableDataSource>
    {
        /// <summary>
        /// The unique identifier of the user within the third-party identity provider.
        /// </summary>
        [Required]
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// The calling user's identity provider.
        /// </summary>
        [StringLength(32, ErrorMessage = "Identity provider name must be less than 32 characters long.", MinimumLength = 1)]
        public string IdentityProvider { get; set; }

        /// <summary>
        /// The user's personal name.
        /// </summary>
        [StringLength(128, ErrorMessage = "Name must be less than 128 characters long.", MinimumLength = 1)]
        public string ExternalIdentityName { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [StringLength(256, ErrorMessage = "Email address must be less than 256 characters long.", MinimumLength = 1)]
        public string ExternalIdentityEmailAddress { get; set; }

        public GetCurrentUser(ILogger logger, string externalIdentifier, string identityProvider, string externalIdentityName, string externalIdentityEmailAddress)
            : base(logger)
        {
            ExternalIdentifier = externalIdentifier;
            IdentityProvider = identityProvider;
            ExternalIdentityName = externalIdentityName;
            ExternalIdentityEmailAddress = externalIdentityEmailAddress;
        }

        protected override async Task<User> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var user = await databaseConnection.QuerySingleOrDefaultAsync<User>("security.usp_GetUserByIdentifier",
                new { External_Identifier = ExternalIdentifier, Identity_Provider = IdentityProvider, External_Name = ExternalIdentityName, Email_Address = ExternalIdentityEmailAddress },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return user;
        }
    }
}

using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Http.Authentication
{
    internal class CurrentUserRetriever : ICurrentUserRetriever
    {
        private const string ExternalIdentifierClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private const string IdentityProviderClaim = "http://schemas.microsoft.com/identity/claims/identityprovider";
        private const string NameClaim = "name";

        private readonly ILogger<CurrentUserRetriever> _logger;
        private readonly HttpContext _httpContext;
        private readonly SqlServerDataSource _dataSource;

        public CurrentUserRetriever(ILogger<CurrentUserRetriever> logger, IHttpContextAccessor httpContextAccessor, SqlServerDataSource dataSource)
        {
            _logger = logger;
            _httpContext = httpContextAccessor?.HttpContext;
            _dataSource = dataSource;
        }

        public Task<User> GetCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        public ExternalIdentity GetExternalIdentity()
        {
            var currentUser = _httpContext?.User;
            var externalIdentifier = currentUser?.FindFirst(ExternalIdentifierClaim)?.Value;
            var identityProvider = currentUser?.FindFirst(IdentityProviderClaim)?.Value;

            if (string.IsNullOrWhiteSpace(externalIdentifier)
                | string.IsNullOrWhiteSpace(identityProvider))
                return null;

            return new ExternalIdentity()
            {
                ExternalIdentifier = externalIdentifier,
                Provider = identityProvider
            };
        }
    }
}

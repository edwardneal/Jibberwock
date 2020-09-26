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
        private const string EmailAddressClaim = "emails";

        private object _cachedUserLock;
        private User _cachedUser;

        private readonly ILogger<CurrentUserRetriever> _logger;
        private readonly HttpContext _httpContext;
        private readonly SqlServerDataSource _dataSource;

        public CurrentUserRetriever(ILogger<CurrentUserRetriever> logger, IHttpContextAccessor httpContextAccessor, SqlServerDataSource dataSource)
        {
            _cachedUserLock = new object();
            _logger = logger;
            _httpContext = httpContextAccessor?.HttpContext;
            _dataSource = dataSource;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            User localCachedUser;

            lock (_cachedUserLock)
            { localCachedUser = _cachedUser; }
            
            if (localCachedUser == null)
            {
                // Get the current ExternalIdentity, augment it with the current name and email address, and fire a GetCurrentUser command off to the database
                var externalIdentity = GetExternalIdentity();
                var currentUser = _httpContext?.User;
                var name = currentUser?.FindFirst(NameClaim)?.Value;
                var emailAddress = currentUser?.FindFirst(EmailAddressClaim)?.Value;

                // A Name claim is mandatory, but an email address is not. This is because a GitHub login doesn't return it
                if (!string.IsNullOrEmpty(name))
                {
                    var getCurrentUserCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetCurrentUser(_logger, externalIdentity.ExternalIdentifier, externalIdentity.Provider, name, emailAddress);

                    // If this is null, the user doesn't exist in the database. We'll pretend that they do though, so we can run our permissions checks
                    localCachedUser = await getCurrentUserCommand.Execute(_dataSource)
                        ?? new User() { Id = 0, Name = name, EmailAddress = emailAddress };

                    lock (_cachedUserLock)
                    { _cachedUser = localCachedUser; }
                }
            }

            return localCachedUser;
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

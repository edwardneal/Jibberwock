using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Http.Authentication
{
    /// <summary>
    /// Enables the current Jibberwock user to be retrieved. The current user is cached per request to reduce database access.
    /// </summary>
    public interface ICurrentUserRetriever
    {
        /// <summary>
        /// Gets the current Jibberwock user.
        /// </summary>
        /// <returns>The current Jibberwock user.</returns>
        Task<User> GetCurrentUserAsync();

        /// <summary>
        /// Gets the current external identity.
        /// </summary>
        /// <remarks>
        /// This does not call the database and is not cached.
        /// </remarks>
        /// <returns>The current external identity.</returns>
        ExternalIdentity GetExternalIdentity();
    }
}

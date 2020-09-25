using Jibberwock.Shared.Http.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Http.Authentication
{
    /// <summary>
    /// Contains extension methods which enable Jibberwock security handlers.
    /// </summary>
    public static class ServiceCollectionAuthenticationExtensions
    {
        /// <summary>
        /// Enables Jibberwock security handlers, enabling the processing of <see cref="ResourcePermissionsAttribute"/>s.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddJibberwockSecurity(this IServiceCollection services)
        {
            return services.AddScoped<ICurrentUserRetriever, CurrentUserRetriever>()
                .AddScoped<IAuthorizationHandler, ResourcePermissionHandler>();
        }
    }
}

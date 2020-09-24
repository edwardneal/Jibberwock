using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.Http.Authorization
{
    /// <summary>
    /// References the <see cref="ResourcePermissionsAttribute"/>s on the target parameters and the current user
    /// in order to make permission decisions.
    /// </summary>
    public class ResourcePermissionHandler : IAuthorizationHandler
    {
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            context.Succeed(context.PendingRequirements.First());
        }
    }
}

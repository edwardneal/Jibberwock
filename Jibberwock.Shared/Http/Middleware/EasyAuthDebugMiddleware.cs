using Jibberwock.Shared.Configuration.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Http.Middleware
{
    /// <summary>
    /// This middleware injects the necessary headers and environment variable to set a particular security principal.
    /// </summary>
    public class EasyAuthDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EasyAuthDebugConfiguration _debugConfiguration;

        public EasyAuthDebugMiddleware(RequestDelegate next, EasyAuthDebugConfiguration debugConfiguration)
        {
            _next = next;
            _debugConfiguration = debugConfiguration;
        }

        public async Task Invoke(HttpContext context)
        {
            Environment.SetEnvironmentVariable("WEBSITE_AUTH_ENABLED", "True");
            context.Request.Headers["X-MS-CLIENT-PRINCIPAL-IDP"] = _debugConfiguration.IdProvider;
            context.Request.Headers["X-MS-CLIENT-PRINCIPAL"] = _debugConfiguration.Principal;
            await _next(context);
        }
    }
}

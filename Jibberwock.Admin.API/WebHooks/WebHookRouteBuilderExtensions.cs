using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.WebHooks
{
    public static class WebHookRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapSendGridWebHooks(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapMethods(SendGrid.SendGridEndpointHandler.EndpointPattern, new[] { HttpMethods.Post }, SendGrid.SendGridEndpointHandler.HandleSendGridWebHook);
        }
    }
}

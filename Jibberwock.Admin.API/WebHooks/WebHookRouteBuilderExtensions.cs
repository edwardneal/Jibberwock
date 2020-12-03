using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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

        public static Stripe.StripeEventHub MapStripeWebHooks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapMethods(Stripe.StripeEndpointHandler.EndpointPattern, new[] { HttpMethods.Post }, Stripe.StripeEndpointHandler.HandleStripeWebHook);

            return endpoints.ServiceProvider.GetRequiredService<Stripe.StripeEventHub>();
        }
    }
}

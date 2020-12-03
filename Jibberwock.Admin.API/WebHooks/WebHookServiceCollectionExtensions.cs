using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.WebHooks
{
    public static class WebHookServiceCollectionExtensions
    {
        public static IServiceCollection AddJibberwockStripeEvents(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<Stripe.StripeEventHub>();
        }
    }
}

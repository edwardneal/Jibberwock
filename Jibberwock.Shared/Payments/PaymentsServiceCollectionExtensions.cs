using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Payments
{
    public static class PaymentsServiceCollectionExtensions
    {
        /// <summary>
        /// Enables the Jibberwock payment libraries to be accessed using dependency injection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The input parameter, with all payment libraries added.</returns>
        public static IServiceCollection AddJibberwockPayments(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddScoped<Payments.IPaymentProvider, Payments.Stripe.StripePaymentProvider>();
        }
    }
}

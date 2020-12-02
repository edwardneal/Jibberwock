using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.ActionModels.Tenants
{
    /// <summary>
    /// Describes the response from <see cref="Jibberwock.Core.API.Controllers.Tenants.TenantController.CreateTenant(TenantCreationOptions)"/>.
    /// </summary>
    public class CreateTenantResponse
    {
        /// <summary>
        /// The internal identifier of the created tenant.
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// The ID of a Stripe session which enables payment of services for this tenant.
        /// </summary>
        public string StripeSessionId { get; set; }

        /// <summary>
        /// The publishable key used to perform the client-side redirect to Stripe for payment.
        /// </summary>
        public string StripePublishableKey { get; set; }

        /// <summary>
        /// If <c>true</c>, the client must redirect to Stripe for payment before one or more subscriptions is activated.
        /// </summary>
        public bool PaymentRequired { get; set; }
    }
}

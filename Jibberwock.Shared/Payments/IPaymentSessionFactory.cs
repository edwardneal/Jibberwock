using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Payments
{
    /// <summary>
    /// Interface describing a payment provider's ability to handle payment sessions.
    /// </summary>
    public interface IPaymentSessionFactory
    {
        /// <summary>
        /// Creates a payment session which results in a subscription.
        /// </summary>
        /// <param name="returnUrlBase">The base for the success/cancellation return URLs.</param>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="productIds">A set of product IDs to be paid for.</param>
        /// <param name="subscriptionMetadata">Any metadata to be associated with the resultant subscription.</param>
        /// <returns>The subscription session ID.</returns>
        /// <remarks>
        /// The subscription's Success URL will be <paramref name="returnUrlBase"/>?result=success&session={{CHECKOUT_SESSION_ID}}.
        /// The subscription's Cancellation URL will be <paramref name="returnUrlBase"/>?result=cancel.
        /// </remarks>
        Task<string> CreateSubscriptionSession(string returnUrlBase, string customerId, IEnumerable<string> productIds, Dictionary<string, string> subscriptionMetadata);
    }
}

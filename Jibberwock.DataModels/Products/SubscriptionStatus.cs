using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products
{
    /// <summary>
    /// Describes the possible statuses of a <see cref="Subscription"/>.
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// The <see cref="Subscription"/> is currently in its trial period.
        /// </summary>
        Trial = 1,
        /// <summary>
        /// The <see cref="Subscription"/> is active. This is its normal status.
        /// </summary>
        Active = 2,
        /// <summary>
        /// The <see cref="Subscription"/> has expired without renewal.
        /// </summary>
        Expired = 3,
        /// <summary>
        /// The <see cref="Subscription"/> is active, but card details have expired and the next invoice cannot be paid.
        /// </summary>
        BillingDetailsExpired = 4,
        /// <summary>
        /// The last invoice for this <see cref="Subscription"/> has not been paid.
        /// </summary>
        Unpaid = 5
    }
}

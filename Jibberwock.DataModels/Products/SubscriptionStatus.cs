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
        /// The <see cref="Subscription"/> is waiting to receive the first payment.
        /// </summary>
        PaymentPending = 1,
        /// <summary>
        /// The <see cref="Subscription"/> is currently in its trial period.
        /// </summary>
        Trial = 2,
        /// <summary>
        /// The <see cref="Subscription"/> is active. This is its normal status.
        /// </summary>
        Active = 3,
        /// <summary>
        /// The <see cref="Subscription"/> has expired without renewal.
        /// </summary>
        Expired = 4,
        /// <summary>
        /// The <see cref="Subscription"/> is active, but card details have expired and the next invoice cannot be paid.
        /// </summary>
        BillingDetailsExpired = 5,
        /// <summary>
        /// The last invoice for this <see cref="Subscription"/> has not been paid.
        /// </summary>
        Unpaid = 6
    }
}

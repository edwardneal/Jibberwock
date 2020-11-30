using Jibberwock.DataModels.Products.Configuration;
using Jibberwock.DataModels.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products
{
    /// <summary>
    /// A <see cref="Subscription"/> represents a link between a <see cref="Tenant"/> and a <see cref="Tier"/>.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// The unique identifier for this <see cref="Subscription"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The unique identifier of this <see cref="Subscription"/> in the external billing system.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Determines whether or not this <see cref="Subscription"/> takes effect.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The unique identifier of the most recent invoice for this <see cref="Subscription"/> in the external billing system.
        /// </summary>
        public string LastInvoiceExternalId { get; set; }

        /// <summary>
        /// The <see cref="Tenant"/> which this <see cref="Subscription"/> is active for.
        /// </summary>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// The <see cref="Tier"/> which this <see cref="Subscription"/> is subscribed to.
        /// </summary>
        public Tier ProductTier { get; set; }

        /// <summary>
        /// All <see cref="Product"/>-specific configuration for this subscription (typically, tenant-level settings.)
        /// </summary>
        public ProductConfigurationBase Configuration { get; set; }

        /// <summary>
        /// First date that this <see cref="Subscription"/> takes effect.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// If present, the final date that this <see cref="Subscription"/> has effect.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// The current status of this <see cref="Subscription"/>.
        /// </summary>
        public SubscriptionStatus Status { get; set; }
    }
}

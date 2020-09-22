using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products
{
    /// <summary>
    /// A billing tier for a specific <see cref="Product"/>.
    /// </summary>
    public class Tier
    {
        /// <summary>
        /// A unique identifier for this <see cref="Tier"/>.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique identifier of this <see cref="Tier"/> in the external billing system.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The <see cref="Product"/> this <see cref="Tier"/> is associated with.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Determines whether or not this <see cref="Tier"/> appears in lists or on the homepage.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// If present, the first date that this <see cref="Tier"/> is available for purchase.
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// If present, the final date that this <see cref="Tier"/> is available for purchase.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// This <see cref="Tier"/>'s characteristics - one record for every item in <see cref="Product.AvailableCharacteristics"/>.
        /// </summary>
        public IEnumerable<TierProductCharacteristic> Characteristics { get; set; }
    }
}

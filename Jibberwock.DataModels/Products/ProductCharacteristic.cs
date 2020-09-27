using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products
{
    /// <summary>
    /// A specific characteristic of one or more <see cref="Product"/>s.
    /// </summary>
    public class ProductCharacteristic
    {
        /// <summary>
        /// A unique identifier for this <see cref="ProductCharacteristic"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of this <see cref="ProductCharacteristic"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If present, the long-form description of this <see cref="ProductCharacteristic"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Determines whether or not this <see cref="ProductCharacteristic"/> should appear on feature lists or in billing tier comparisons.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Determines whether or not this <see cref="ProductCharacteristic"/> is available to add to billing tiers.
        /// </summary>
        public bool Enabled { get; set; }
    }
}

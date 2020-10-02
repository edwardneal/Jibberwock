using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products
{
    /// <summary>
    /// A specific characteristic of a given <see cref="Tier"/>.
    /// </summary>
    public class TierProductCharacteristic
    {
        /// <summary>
        /// A unique identifier for this <see cref="TierProductCharacteristic"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The <see cref="Tier"/> which this <see cref="TierProductCharacteristic"/> is associated with.
        /// </summary>
        public Tier Tier { get; set; }

        /// <summary>
        /// The global <see cref="ProductCharacteristic"/> which this <see cref="TierProductCharacteristic"/> is providing a value for.
        /// </summary>
        public ProductCharacteristic ProductCharacteristic { get; set; }

        /// <summary>
        /// The value of this <see cref="ProductCharacteristic"/> for this <see cref="Tier"/>.
        /// </summary>
        /// <remarks>
        /// This could have a handful of different values, but these will usually be <see cref="string"/>, <see cref="long"/> or <see cref="bool"/>.
        /// </remarks>
        public object CharacteristicValue { get; set; }
    }
}

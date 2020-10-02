using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products
{
    /// <summary>
    /// Describes the type of values which can be stored for a <see cref="ProductCharacteristic"/> in a <see cref="TierProductCharacteristic"/>.
    /// </summary>
    public enum ProductCharacteristicValueType
    {
        /// <summary>
        /// This <see cref="ProductCharacteristic"/> can store textual strings.
        /// </summary>
        String = 1,
        /// <summary>
        /// This <see cref="ProductCharacteristic"/> can store boolean <c>true</c>/<c>false</c>.
        /// </summary>
        Boolean = 2,
        /// <summary>
        /// This <see cref="ProductCharacteristic"/> can store numeric data.
        /// </summary>
        Numeric = 3
    }
}

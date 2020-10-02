using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// These are the possible return values returned by executing <see cref="DeleteCharacteristic"/>.
    /// </summary>
    public enum DeleteCharacteristicStatusCode
    {
        /// <summary>
        /// The <see cref="ProductCharacteristic"/> was deleted successfully.
        /// </summary>
        Success = 0,
        /// <summary>
        /// The <see cref="ProductCharacteristic"/> is associated with a <see cref="Tier"/>
        /// </summary>
        AssociatedTier = 1,
        /// <summary>
        /// The <see cref="ProductCharacteristic"/> is associated with a <see cref="Product"/>.
        /// </summary>
        AssociatedProduct = 2,
        /// <summary>
        /// No <see cref="ProductCharacteristic"/> with this ID exists.
        /// </summary>
        Missing = 3
    }
}

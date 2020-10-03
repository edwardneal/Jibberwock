using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Products
{
    /// <summary>
    /// Describes the settings sent to <see cref="Jibberwock.Admin.API.Controllers.Products.ProductCharacteristicController.UpdateProductCharacteristic(string, ProductCharacteristicChangeSetting)"/>
    /// or to <see cref="Jibberwock.Admin.API.Controllers.Products.ProductCharacteristicController.CreateCharacteristic(ProductCharacteristicChangeSetting)"/>.
    /// </summary>
    public class ProductCharacteristicChangeSetting
    {
        /// <summary>
        /// The new name of the product characteristic.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new long-form description of the product characteristic.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The visibility of the product characteristic. If <c>false</c>, the product characteristic will not appear on feature lists or in billing tier comparisons.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The enabled state of the product characteristic. If <c>false</c>, it will not be available to add to a billing tier.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The acceptable type of all values associated with this product characteristic.
        /// </summary>
        public ProductCharacteristicValueType ValueType { get; set; }
    }
}

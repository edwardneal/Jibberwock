using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Products
{
    /// <summary>
    /// Describes the data sent to <see cref="Jibberwock.Admin.API.Controllers.Products.ProductController.CreateProduct(ProductCreationOptions)"/> or to
    /// <see cref="Jibberwock.Admin.API.Controllers.Products.ProductController.UpdateProduct(long, ProductCreationOptions)"/>.
    /// </summary>
    public class ProductCreationOptions
    {
        /// <summary>
        /// The new name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The new description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The new location of the product's dedicated information page.
        /// </summary>
        public string MoreInformationUrl { get; set; }

        /// <summary>
        /// The visible state of the product. If <c>false</c>, it will not appear in a product list.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The new, complete list of all applicable characteristics. Sending this will overwrite the previous list; invalid characteristic IDs will be ignored.
        /// </summary>
        public IEnumerable<int> ApplicableCharacteristicIDs { get; set; }
    }
}

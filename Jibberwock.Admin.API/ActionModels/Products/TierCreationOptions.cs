using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Products
{
    /// <summary>
    /// Describes the settings sent to <see cref="Jibberwock.Admin.API.Controllers.Products.ProductController.CreateProductTier(long, TierCreationOptions)"/> and
    /// <see cref="Jibberwock.Admin.API.Controllers.Products.ProductController.UpdateProductTier(long, long, TierCreationOptions)"/>.
    /// </summary>
    public class TierCreationOptions
    {
        /// <summary>
        /// The unique identifier of the tier in the external billing system.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The friendly name of this tier.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If true, this tier will appear in lists and on the homepage.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The first date that this tier is available for use.
        /// <c>null</c> if this should not restrict its availability.
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// The last date that this tier is available for use.
        /// <c>null</c> if this should not restrict its availability.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// This tier's characteristics.
        /// </summary>
        public IEnumerable<TierCharacteristicValue> CharacteristicValues { get; set; }
    }
}

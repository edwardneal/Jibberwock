using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products.Configuration
{
    /// <summary>
    /// This base class stores any <see cref="Product"/>-specific configuration for a specific <see cref="Subscription"/>.
    /// </summary>
    public abstract class ProductConfigurationBase
    {
        /// <summary>
        /// The unique identifier for this <see cref="ProductConfigurationBase"/> derivate.
        /// </summary>
        public Guid Id { get; set; }
    }
}

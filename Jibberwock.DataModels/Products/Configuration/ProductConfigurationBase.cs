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
        public long Id { get; set; }

        /// <summary>
        /// The raw, untyped configuration string which contains the data for this <see cref="ProductConfigurationBase"/> derivate.
        /// </summary>
        public abstract string ConfigurationString { get; set; }

        public ProductConfigurationBase(ProductConfigurationBase sourceConfiguration)
        {
            this.Id = sourceConfiguration.Id;
            this.ConfigurationString = sourceConfiguration.ConfigurationString;
        }

        protected ProductConfigurationBase() { }
    }
}

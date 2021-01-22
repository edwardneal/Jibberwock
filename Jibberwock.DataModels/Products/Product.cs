using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products
{
    /// <summary>
    /// A product or service which is available to be offered to organisations.
    /// </summary>
    public class Product : SecurableResource
    {
        /// <summary>
        /// The friendly, public-facing name of this <see cref="Product"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Long description of this <see cref="Product"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// If populated, a link to this <see cref="Product"/>'s dedicated information page.
        /// </summary>
        public string MoreInformationUrl { get; set; }

        /// <summary>
        /// Determines whether or not this <see cref="Product"/> appears in lists or on the homepage.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The default configuration for this <see cref="Product"/>.
        /// </summary>
        public Configuration.ProductConfigurationBase DefaultProductConfiguration { get; set; }

        /// <summary>
        /// Name of the control used by the front-end to configure this <see cref="Product"/> on a <see cref="Subscription"/>.
        /// </summary>
        public string ConfigurationControlName { get; set; }

        /// <summary>
        /// All <see cref="ProductCharacteristic"/>s available to be linked to this <see cref="Product"/>'s billing tiers.
        /// </summary>
        public IEnumerable<ProductCharacteristic> ApplicableCharacteristics { get; set; }

        /// <summary>
        /// All available <see cref="Tier"/>s for this <see cref="Product"/>.
        /// </summary>
        public IEnumerable<Tier> Tiers { get; set; }
    }
}

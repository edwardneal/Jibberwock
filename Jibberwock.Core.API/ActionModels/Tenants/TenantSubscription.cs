using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.ActionModels.Tenants
{
    /// <summary>
    /// Describes the information required to subscribe to a new product (or a new product's tier)
    /// </summary>
    public class TenantSubscription
    {
        /// <summary>
        /// The ID of the new product tier.
        /// </summary>
        public long ProductTierId { get; set; }

        /// <summary>
        /// A product-specific configuration string (usually a JSON string)
        /// </summary>
        public string ProductConfiguration { get; set; }
    }
}

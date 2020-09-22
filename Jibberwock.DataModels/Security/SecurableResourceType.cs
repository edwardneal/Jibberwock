using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// Describes the type of a <see cref="SecurableResource"/>.
    /// </summary>
    public enum SecurableResourceType
    {
        /// <summary>
        /// This <see cref="SecurableResource"/> is a <see cref="Tenant"/>.
        /// </summary>
        Tenant = 1,
        /// <summary>
        /// This <see cref="SecurableResource"/> is a <see cref="ApiKey"/>.
        /// </summary>
        ApiKey = 2,
        /// <summary>
        /// This <see cref="SecurableResource"/> is a <see cref="Product"/>.
        /// </summary>
        Product = 3
    }
}

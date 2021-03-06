﻿using Jibberwock.DataModels.Products;
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
        /// This <see cref="SecurableResource"/> is a <see cref="Jibberwock.DataModels.Tenants.Tenant"/>.
        /// </summary>
        Tenant = 1,
        /// <summary>
        /// This <see cref="SecurableResource"/> is a <see cref="Jibberwock.DataModels.Tenants.ApiKey"/>.
        /// </summary>
        ApiKey = 2,
        /// <summary>
        /// This <see cref="SecurableResource"/> is a <see cref="Jibberwock.DataModels.Products.Product"/>.
        /// </summary>
        Product = 3,
        /// <summary>
        /// This <see cref="SecurableResource"/> is a <see cref="Jibberwock.DataModels.Core.Service"/>.
        /// </summary>
        Service = 4,
        /// <summary>
        /// This <see cref="SecurableResource"/> is an <see cref="Jibberwock.DataModels.Allert.AlertDefinition"/>.
        /// </summary>
        Allert_AlertDefinition = 5,
        /// <summary>
        /// This <see cref="SecurableResource"/> is an <see cref="Jibberwock.DataModels.Allert.AlertDefinitionGroup"/>.
        /// </summary>
        Allert_AlertDefinitionGroup = 6
    }
}

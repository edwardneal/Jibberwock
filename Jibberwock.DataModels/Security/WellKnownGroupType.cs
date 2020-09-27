using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// Well known groups for a <see cref="Tenant"/> or <see cref="Service"/>.
    /// </summary>
    public enum WellKnownGroupType
    {
        /// <summary>
        /// This group contains all administrators of this <see cref="Service"/>.
        /// </summary>
        ServiceAdministrators = 1,
        /// <summary>
        /// This group contains all members capable of adding new <see cref="Jibberwock.DataModels.Products.Product"/> to this <see cref="Service"/>.
        /// </summary>
        ProductAdministrators = 2,
        /// <summary>
        /// This group contains all auditors of this <see cref="Tenant"/> or <see cref="Service"/>.
        /// </summary>
        Auditors = 3,
        /// <summary>
        /// This group contains all members capable of reading <see cref="Service"/>-level data.
        /// </summary>
        ServiceReaders = 4,
        /// <summary>
        /// This group contains every member capable of changing the billing details of this <see cref="Tenant"/>, deleting it or subscribing/cancelling <see cref="Product"/> subscriptions.
        /// </summary>
        BillingAdministrators = 5,
        /// <summary>
        /// This group contains all members of this <see cref="Tenant"/>.
        /// </summary>
        TenantMembers = 6,
        /// <summary>
        /// This group contains all administrators of this <see cref="Tenant"/>.
        /// </summary>
        TenantAdministrators = 7,
        /// <summary>
        /// This group contains every member capable of seeing or modifying <see cref="ApiKey"/>s for this <see cref="Tenant"/>.
        /// </summary>
        ApiKeyAdministrators = 8
    }
}

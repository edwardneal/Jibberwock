using Jibberwock.DataModels.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// Well known groups for a <see cref="Tenant"/>.
    /// </summary>
    public enum WellKnownGroupType
    {
        /// <summary>
        /// This group contains all members of this <see cref="Tenant"/>.
        /// </summary>
        TenantMembers = 1,
        /// <summary>
        /// This group contains all administrators of this <see cref="Tenant"/>.
        /// </summary>
        TenantAdministrators = 2,
        /// <summary>
        /// This group contains every member capable of changing the billing details of this <see cref="Tenant"/>, deleting it or subscribing/cancelling <see cref="Product"/> subscriptions.
        /// </summary>
        BillingAdministrators = 3,
        /// <summary>
        /// This group contains every member capable of seeing or modifying <see cref="ApiKey"/>s for this <see cref="Tenant"/>.
        /// </summary>
        ApiKeyAdministrators = 4
    }
}

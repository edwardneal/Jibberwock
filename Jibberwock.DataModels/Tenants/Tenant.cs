using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Tenants
{
    /// <summary>
    /// A <see cref="Tenant"/> represents a single organisation, with various subscriptions, API keys and product-specific properties.
    /// </summary>
    public class Tenant : SecurableResource
    {
        /// <summary>
        /// The unique internal reference for this <see cref="Tenant"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The preferred name of this <see cref="Tenant"/>. This will be the name which is displayed for a user when they log in.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Point of contact for any <see cref="Tenant"/>-specific billing information, invoices, etc.
        /// </summary>
        public Contact BillingContact { get; set; }

        /// <summary>
        /// The permissions (and associated <see cref="Group"/>s) granted over this <see cref="Tenant"/>.
        /// </summary>
        public IEnumerable<AccessControlEntry> AccessControlEntries { get; set; }

        /// <summary>
        /// All "well-known <see cref="Group"/>s" for this <see cref="Tenant"/>. This provides a hard link to specific tenant-level groups.
        /// </summary>
        public IReadOnlyDictionary<WellKnownGroupType, Group> WellKnownGroups { get; set; }

        /// <summary>
        /// API keys available for any <see cref="Product"/>s which are part of this <see cref="Tenant"/>.
        /// </summary>
        public IEnumerable<ApiKey> ApiKeys { get; set; }
    }
}

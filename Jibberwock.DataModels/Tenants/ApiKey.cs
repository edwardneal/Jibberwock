using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Security;
using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Tenants
{
    /// <summary>
    /// Represents an API key used to access a service.
    /// </summary>
    public class ApiKey : SecurableResource
    {
        /// <summary>
        /// Determines if this <see cref="ApiKey"/> can be used. This can be used to immediately lock down a specific service.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Date that this <see cref="ApiKey"/> was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// If set, the date that this <see cref="ApiKey"/> becomes usable.
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// If set, the date that this <see cref="ApiKey"/> becomes unusable.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// The <see cref="Tenant"/> which this <see cref="ApiKey"/> provides access to <see cref="Product"/>s of.
        /// </summary>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// The <see cref="Product"/> which this <see cref="ApiKey"/> is associated with.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// The "virtual <see cref="User"/>" record. This <see cref="ApiKey"/> has the permissions associated with this <see cref="User"/>.
        /// </summary>
        public User VirtualUser { get; set; }
    }
}

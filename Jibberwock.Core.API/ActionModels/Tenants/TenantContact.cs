using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.ActionModels.Tenants
{
    /// <summary>
    /// Describes the contact details used when creating a specific tenant.
    /// </summary>
    public class TenantContact
    {
        /// <summary>
        /// The contact person's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A contact email address for the tenant.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// A contact phone number for the tenant.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// If <c>true</c>, the current user's name and email address will populate the <see cref="Name"/> and <see cref="EmailAddress"/> fields.
        /// </summary>
        public bool UseOwnDetails { get; set; }
    }
}

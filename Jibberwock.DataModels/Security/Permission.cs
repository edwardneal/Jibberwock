using Jibberwock.DataModels.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// This is a possible permission which can be provided to a securable object as part of an <see cref="AccessControlEntry"/>.
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// Read the details of this specific securable object.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Update non-sensitive fields.
        /// </summary>
        Change = 2,
        /// <summary>
        /// Update the billing contact for the <see cref="Tenant"/>.
        /// </summary>
        ChangeBillingContact = 3,
        /// <summary>
        /// Change or cancel a subscription for the <see cref="Tenant"/>.
        /// </summary>
        ChangeSubscriptionBilling = 4,
        /// <summary>
        /// Delete this specific securable object.
        /// </summary>
        Delete = 5,
        /// <summary>
        /// Invite someone to the <see cref="Tenant"/> or service.
        /// </summary>
        Invite = 6,
        /// <summary>
        /// View <see cref="Tenant"/> or service-specific audit and activity logs.
        /// </summary>
        ReadLogs = 7,
        /// <summary>
        /// Create a new API key for a <see cref="Product"/>.
        /// </summary>
        CreateApiKey = 8
    }
}

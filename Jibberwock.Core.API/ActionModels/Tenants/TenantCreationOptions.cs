using Jibberwock.Core.API.ActionModels.Invitations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.ActionModels.Tenants
{
    /// <summary>
    /// Describes the settings sent to <see cref="Jibberwock.Core.API.Controllers.Tenants.TenantController.CreateTenant(TenantCreationOptions)"/>.
    /// </summary>
    public class TenantCreationOptions
    {
        /// <summary>
        /// The new tenant's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The contact details of the tenant.
        /// </summary>
        public TenantContact Contact { get; set; }

        /// <summary>
        /// A list of invitations to be sent to the new tenant.
        /// </summary>
        public IEnumerable<InvitationRequest> Invitations { get; set; }

        /// <summary>
        /// A list of the new tenant's subscriptions.
        /// </summary>
        public IEnumerable<TenantSubscription> Subscriptions { get; set; }
    }
}

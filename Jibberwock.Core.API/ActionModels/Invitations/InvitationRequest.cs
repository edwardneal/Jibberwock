using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.ActionModels.Invitations
{
    /// <summary>
    /// Describes the information sent to the Jibberwock API to invite somebody to a tenant.
    /// </summary>
    public class InvitationRequest
    {
        /// <summary>
        /// The email address of the person to invite.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The IdP which the person's user account should log in to.
        /// </summary>
        public string IdentityProvider { get; set; }

        /// <summary>
        /// If <c>true</c>, an invitation email will be sent to <see cref="EmailAddress"/>.
        /// </summary>
        public bool SendEmail { get; set; }
    }
}

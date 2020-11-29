using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Tenants
{
    /// <summary>
    /// Represents the status of an <see cref="Invitation"/>.
    /// </summary>
    public enum InvitationStatus
    {
        /// <summary>
        /// This <see cref="Invitation"/> is active.
        /// </summary>
        Active = 1,
        /// <summary>
        /// This <see cref="Invitation"/> has been revoked.
        /// </summary>
        Revoked = 2
    }
}

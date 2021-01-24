using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Users
{
    /// <summary>
    /// Describes the type of <see cref="User"/> record.
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// This <see cref="User"/> is a normal user account.
        /// </summary>
        UserAccount = 1,
        /// <summary>
        /// This <see cref="User"/> is associated with an invitation. Permissions associated with
        /// this <see cref="User"/> will be transferred to another <see cref="User"/> if they accept
        /// their invitation.
        /// </summary>
        InvitationPlaceholder = 2,
        /// <summary>
        /// This <see cref="User"/> is associated with an API key. Permissions associated with this
        /// <see cref="User"/> will be granted to API keys.
        /// </summary>
        ApiPrincipal = 3
    }
}

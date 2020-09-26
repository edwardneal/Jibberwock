using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Users
{
    /// <summary>
    /// An external user account which logs in and is linked to an internal
    /// Jibberwock <see cref="User"/>.
    /// </summary>
    public class ExternalIdentity
    {
        /// <summary>
        /// The unique internal reference for this <see cref="ExternalIdentity"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// This is the provider-specific external identifier.
        /// </summary>
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// The authentication source of this <see cref="ExternalIdentity"/>.
        /// </summary>
        public string Provider { get; set; }
    }
}

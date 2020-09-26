using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Users
{
    /// <summary>
    /// A person who logs in and is given access to at least one <see cref="Tenant"/>.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique internal reference for this <see cref="User"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// This <see cref="User"/>'s name according to the authentication source.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This <see cref="User"/>'s email address according to the authentication source.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The external accounts which map to this unique Jibberwock <see cref="User"/>.
        /// </summary>
        public IEnumerable<ExternalIdentity> ExternalIdentities { get; set; }
    }
}

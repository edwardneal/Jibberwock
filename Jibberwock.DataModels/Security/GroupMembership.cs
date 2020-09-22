using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// Cross-references a <see cref="Group"/> to a <see cref="User"/>.
    /// </summary>
    public class GroupMembership
    {
        /// <summary>
        /// The unique internal reference for this <see cref="GroupMembership"/>.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Determines whether or not this <see cref="GroupMembership"/> takes effect.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The <see cref="Group"/> which <see cref="User"/> is a member of.
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// The <see cref="User"/> which <see cref="Group"/> contains.
        /// </summary>
        public User User { get; set; }
    }
}

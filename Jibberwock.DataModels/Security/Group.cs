using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// This is a set of <see cref="User"/>s, with specific permissions over a set of resources, a tenant or Jibberwock as a whole
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The unique internal reference for this <see cref="Group"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The visible name of this <see cref="Group"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional reference to the <see cref="Tenant"/> which contains the users who can be members of this <see cref="Group"/>
        /// </summary>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// If this <see cref="Group"/> is a well-known group, its type. <c>null</c> if this is a custom group.
        /// </summary>
        public WellKnownGroupType? WellKnownGroupType { get; set; }

        /// <summary>
        /// Members of this <see cref="Group"/>
        /// </summary>
        public IEnumerable<GroupMembership> Users { get; set; }
    }
}

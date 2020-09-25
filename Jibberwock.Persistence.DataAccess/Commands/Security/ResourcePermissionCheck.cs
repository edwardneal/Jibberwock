using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Describes a set of permission checks to be made against a specific <see cref="SecurableResource"/>.
    /// </summary>
    public class ResourcePermissionCheck
    {
        /// <summary>
        /// The ID of the <see cref="SecurableResource"/> to check.
        /// </summary>
        /// <remarks>
        /// This might be either a <see cref="Guid"/> or an <see cref="int"/>, depending on the resource type.
        /// </remarks>
        public object ResourceId { get; set; }

        /// <summary>
        /// The type of the <see cref="SecurableResource"/> to check.
        /// </summary>
        public SecurableResourceType ResourceType { get; set; }

        /// <summary>
        /// A list of <see cref="Permission"/>s which the user must have over the <see cref="SecurableResource"/>.
        /// </summary>
        public IEnumerable<Permission> PermissionsRequired { get; set; }
    }
}

using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.Http.Authorization
{
    /// <summary>
    /// Applying this attribute to a parameter in a controller allows the authorization handler to determine which resources
    /// to check permissions of.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ResourcePermissionsAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ResourcePermissionsAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">The type of <see cref="SecurableResource"/> which this parameter refers to the ID of.</param>
        /// <param name="permissionsRequired">The permissions required over the identified <see cref="SecurableResource"/></param>
        public ResourcePermissionsAttribute(SecurableResourceType resourceType, params Permission[] permissionsRequired)
            : base()
        {
            ResourceType = resourceType;
            PermissionsRequired = permissionsRequired;
        }

        /// <summary>
        /// The type of <see cref="SecurableResource"/> which this parameter refers to the ID of.
        /// </summary>
        public SecurableResourceType ResourceType { get; set; }

        /// <summary>
        /// The permissions required over the identified <see cref="SecurableResource"/>.
        /// </summary>
        public IEnumerable<Permission> PermissionsRequired { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// Any classes which derive from this can be targets of an <see cref="AccessControlEntry"/>.
    /// </summary>
    public abstract class SecurableResource
    {
        /// <summary>
        /// Describes the derived subclass type, used by API clients to determine the type of this resource.
        /// </summary>
        public SecurableResourceType ResourceType { get; set; }

        /// <summary>
        /// Describes a quasi-friendly identifier for this resource.
        /// </summary>
        /// <remarks>
        /// Although this should be treated and compared as a string, it will usually be in the format {Type:3 characters}-{incrementing id}
        /// </remarks>
        public string ResourceIdentifier { get; set; }
    }
}

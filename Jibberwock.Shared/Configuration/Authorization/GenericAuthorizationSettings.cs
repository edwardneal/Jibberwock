using Jibberwock.Shared.Http.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration.Authorization
{
    /// <summary>
    /// Contains the configuration elements to support <see cref="ResourcePermissionHandler"/>.
    /// </summary>
    public class GenericAuthorizationSettings
    {
        /// <summary>
        /// The ID of this web API or product in the Jibberwock database. Used to manage permissions over the service as a whole.
        /// </summary>
        public int DefaultServiceId { get; set; }
    }
}

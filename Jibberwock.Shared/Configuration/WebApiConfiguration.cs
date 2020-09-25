using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration
{
    /// <summary>
    /// Contains the top-level configuration for a web API.
    /// </summary>
    public class WebApiConfiguration
    {
        /// <summary>
        /// Configuration for the local debug settings of Azure App Services authentication.
        /// </summary>
        public Authorization.EasyAuthDebugConfiguration EasyAuth { get; set; }

        /// <summary>
        /// Configuration for the service-level settings for permissions enforcement.
        /// </summary>
        public Authorization.GenericAuthorizationSettings Authorization { get; set; }
    }
}

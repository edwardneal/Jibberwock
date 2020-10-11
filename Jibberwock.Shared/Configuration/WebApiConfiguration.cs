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

        /// <summary>
        /// Dictionary which maps a specific token to a URL. Used when redirecting from EasyAuth.
        /// </summary>
        public Dictionary<string, string> PermittedRedirects { get; set; }

        /// <summary>
        /// Configuration for the Azure Service Bus connections.
        /// </summary>
        public ServiceBusConfiguration ServiceBus { get; set; }

        /// <summary>
        /// Configuration for SendGrid web hooks and clients.
        /// </summary>
        public SendGridConfiguration SendGrid { get; set; }
    }
}

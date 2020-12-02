using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration
{
    /// <summary>
    /// Contains configuration for Stripe APIs and web hook processing.
    /// </summary>
    public class StripeConfiguration
    {
        /// <summary>
        /// The API key used for all web API requests.
        /// </summary>
        public string SecretApiKey { get; set; }

        /// <summary>
        /// The API key which handles front-end requests.
        /// </summary>
        public string PublishableApiKey { get; set; }

        /// <summary>
        /// The secret used to validate the legitimacy of web hooks.
        /// </summary>
        public string WebHookSecret { get; set; }
    }
}

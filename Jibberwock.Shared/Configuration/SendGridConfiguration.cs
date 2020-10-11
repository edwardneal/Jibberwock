using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration
{
    /// <summary>
    /// Contains all configuration required to send emails using SendGrid and process web hooks from them.
    /// </summary>
    public class SendGridConfiguration
    {
        /// <summary>
        /// The API key used to make requests and send emails.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// This public key is used to verify web hooks to confirm that they've been sent by SendGrid.
        /// </summary>
        public string VerificationPublicKey { get; set; }

        /// <summary>
        /// This is the name of the parameter used to store the notification ID.
        /// </summary>
        public string NotificationIdParameterName { get; set; }
    }
}

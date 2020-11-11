using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Notifications
{
    /// <summary>
    /// This is an event which occurred on an <see cref="Jibberwock.DataModels.Core.Email"/> record.
    /// </summary>
    public class EmailEvent
    {
        /// <summary>
        /// The type of event, as defined by the external provider.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The ID of the email message as it hit the destination system.
        /// </summary>
        public string SmtpMessageId { get; set; }

        /// <summary>
        /// The timestamp of this event.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// If the email message bounced and this is an appropriate event, the reason for the SMTP bounce.
        /// </summary>
        public string SmtpBounceReason { get; set; }

        /// <summary>
        /// If the email message bounced and this is an appropriate event, the type of SMTP bounce.
        /// </summary>
        public string SmtpBounceType { get; set; }

        /// <summary>
        /// If the email message was dropped and this is an appropriate event, the reason for the SMTP drop.
        /// </summary>
        public string SmtpDroppedReason { get; set; }

        /// <summary>
        /// If the email message was deferred and this is an appropriate event, the SMTP response indicating that this message was deferred.
        /// </summary>
        public string SmtpDeferredResponse { get; set; }
    }
}

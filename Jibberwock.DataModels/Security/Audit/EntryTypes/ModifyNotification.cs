using Jibberwock.DataModels.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies a <see cref="Notification"/>
    /// </summary>
    public class ModifyNotification : AuditTrailEntry
    {
        public ModifyNotification()
            : base()
        {
            Type = AuditTrailEntryType.ModifyNotification;
        }

        /// <summary>
        /// The new state of the <see cref="Notification"/> being modified or created.
        /// </summary>
        public Notification Notification { get; set; }

        /// <summary>
        /// Whether or not to send this <see cref="Notification"/> as an email.
        /// </summary>
        public bool SendAsEmail { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="Notification"/> is a new record.
        /// </summary>
        public bool NewNotification { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { Notification, NewNotification, SendAsEmail });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewNotification = jsonDoc.RootElement.GetProperty(nameof(NewNotification)).GetBoolean();
                Notification = JsonSerializer.Deserialize<Notification>(jsonDoc.RootElement.GetProperty(nameof(Notification)).GetRawText());
                SendAsEmail = jsonDoc.RootElement.GetProperty(nameof(SendAsEmail)).GetBoolean();
            }
        }
    }
}

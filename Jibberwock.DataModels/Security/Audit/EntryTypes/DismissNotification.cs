using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which dismisses a <see cref="Notification"/>.
    /// </summary>
    public class DismissNotification : AuditTrailEntry
    {
        public DismissNotification()
            : base()
        {
            Type = AuditTrailEntryType.DismissNotification;
        }

        /// <summary>
        /// The <see cref="Notification"/> being dismissed.
        /// </summary>
        public Notification Notification { get; set; }


        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { Notification });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                Notification = JsonSerializer.Deserialize<Notification>(jsonDoc.RootElement.GetProperty(nameof(Notification)).GetRawText());
            }
        }
    }
}

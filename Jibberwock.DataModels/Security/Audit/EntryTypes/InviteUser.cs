using Jibberwock.DataModels.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry for a <see cref="User"/>'s invitation to a <see cref="Tenant"/>.
    /// </summary>
    public class InviteUser : AuditTrailEntry
    {
        public InviteUser()
            : base()
        {
            Type = AuditTrailEntryType.InviteUser;
        }

        /// <summary>
        /// If non-null, this is the <see cref="EmailBatch"/> which sends an email.
        /// </summary>
        public EmailBatch EmailBatch { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { EmailBatch });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                EmailBatch = JsonSerializer.Deserialize<EmailBatch>(jsonDoc.RootElement.GetProperty(nameof(EmailBatch)).GetRawText());
            }
        }
    }
}

using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which revokes an <see cref="Tenants.Invitation"/>.
    /// </summary>
    public class RevokeInvitation : AuditTrailEntry
    {
        public RevokeInvitation()
            : base()
        {
            Type = AuditTrailEntryType.RevokeInvitation;
        }

        /// <summary>
        /// The <see cref="Tenants.Invitation"/> being dismissed.
        /// </summary>
        public Invitation Invitation { get; set; }


        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { Invitation });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                Invitation = JsonSerializer.Deserialize<Invitation>(jsonDoc.RootElement.GetProperty(nameof(Invitation)).GetRawText());
            }
        }
    }
}

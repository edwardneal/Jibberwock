using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which deletes an <see cref="AccessControlEntry"/>.
    /// </summary>
    public class DeleteAccessControlEntry : AuditTrailEntry
    {
        public DeleteAccessControlEntry()
            : base()
        {
            Type = AuditTrailEntryType.DeleteAccessControlEntry;
        }

        /// <summary>
        /// The <see cref="AccessControlEntry"/> being deleted.
        /// </summary>
        public AccessControlEntry AccessControlEntry { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { AccessControlEntry });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                AccessControlEntry = JsonSerializer.Deserialize<AccessControlEntry>(jsonDoc.RootElement.GetProperty(nameof(AccessControlEntry)).GetRawText());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies an <see cref="AccessControlEntry"/>.
    /// </summary>
    public class ModifyAccessControlEntry : AuditTrailEntry
    {
        public ModifyAccessControlEntry()
            : base()
        {
            Type = AuditTrailEntryType.ModifyAccessControlEntry;
        }

        /// <summary>
        /// The new state of the <see cref="AccessControlEntry"/> being modified or created.
        /// </summary>
        public AccessControlEntry AccessControlEntry { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="AccessControlEntry"/> is a new record.
        /// </summary>
        public bool NewAccessControlEntry { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { AccessControlEntry, NewAccessControlEntry });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewAccessControlEntry = jsonDoc.RootElement.GetProperty(nameof(NewAccessControlEntry)).GetBoolean();
                AccessControlEntry = JsonSerializer.Deserialize<AccessControlEntry>(jsonDoc.RootElement.GetProperty(nameof(AccessControlEntry)).GetRawText());
            }
        }
    }
}

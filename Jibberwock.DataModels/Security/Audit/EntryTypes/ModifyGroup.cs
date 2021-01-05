using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies a <see cref="Group"/>.
    /// </summary>
    public class ModifyGroup : AuditTrailEntry
    {
        public ModifyGroup()
            : base()
        {
            Type = AuditTrailEntryType.ModifyGroup;
        }

        /// <summary>
        /// The new state of the <see cref="Group"/> being modified or created.
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="Group"/> is a new record.
        /// </summary>
        public bool NewGroup { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { Group, NewGroup });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewGroup = jsonDoc.RootElement.GetProperty(nameof(NewGroup)).GetBoolean();
                Group = JsonSerializer.Deserialize<Group>(jsonDoc.RootElement.GetProperty(nameof(Group)).GetRawText());
            }
        }
    }
}

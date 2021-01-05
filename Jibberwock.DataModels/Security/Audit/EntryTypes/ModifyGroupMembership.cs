using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies a <see cref="GroupMembership"/>.
    /// </summary>
    public class ModifyGroupMembership : AuditTrailEntry
    {
        public ModifyGroupMembership()
            : base()
        {
            Type = AuditTrailEntryType.ModifyGroupMembership;
        }

        /// <summary>
        /// The new state of the <see cref="GroupMembership"/> being modified or created.
        /// </summary>
        public GroupMembership GroupMembership { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="GroupMembership"/> is a new record.
        /// </summary>
        public bool NewGroupMembership { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { GroupMembership, NewGroupMembership });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewGroupMembership = jsonDoc.RootElement.GetProperty(nameof(NewGroupMembership)).GetBoolean();
                GroupMembership = JsonSerializer.Deserialize<GroupMembership>(jsonDoc.RootElement.GetProperty(nameof(GroupMembership)).GetRawText());
            }
        }
    }
}

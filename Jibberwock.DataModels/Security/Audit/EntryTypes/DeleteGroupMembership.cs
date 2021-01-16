using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which deletes a <see cref="GroupMembership"/>.
    /// </summary>
    public class DeleteGroupMembership : AuditTrailEntry
    {
        public DeleteGroupMembership()
            : base()
        {
            Type = AuditTrailEntryType.DeleteGroupMembership;
        }

        /// <summary>
        /// The <see cref="GroupMembership"/> being deleted.
        /// </summary>
        public GroupMembership GroupMembership { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { GroupMembership });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                GroupMembership = JsonSerializer.Deserialize<GroupMembership>(jsonDoc.RootElement.GetProperty(nameof(GroupMembership)).GetRawText());
            }
        }
    }
}

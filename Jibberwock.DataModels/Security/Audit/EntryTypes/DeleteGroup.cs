using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which deletes a <see cref="Security.Group"/>
    /// </summary>
    public class DeleteGroup : AuditTrailEntry
    {
        public DeleteGroup()
            :base()
        {
            Type = AuditTrailEntryType.DeleteGroup;
        }

        /// <summary>
        /// The state of the <see cref="Group"/> being deleted.
        /// </summary>
        public Group Group { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(Group);
            set
            {
                Group = JsonSerializer.Deserialize<Group>(value);
            }
        }
    }
}

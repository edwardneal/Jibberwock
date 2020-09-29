using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which enables or disables a <see cref="User"/>.
    /// </summary>
    public class ControlUserAccess : AuditTrailEntry
    {
        public ControlUserAccess()
            : base()
        {
            Type = AuditTrailEntryType.ControlUserAccess;
        }

        /// <summary>
        /// The new state of <see cref="User.Enabled"/>
        /// </summary>
        public bool Enabled { get; set; }

        public override string Metadata
        {
            get
            {
                return JsonSerializer.Serialize(new { Enabled });
            }
            set
            {
                Enabled = JsonDocument.Parse(value).RootElement.GetProperty(nameof(Enabled)).GetBoolean();
            }
        }
    }
}

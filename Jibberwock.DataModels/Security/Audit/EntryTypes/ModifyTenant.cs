using Jibberwock.DataModels.Tenants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies a <see cref="Tenant"/>.
    /// </summary>
    public class ModifyTenant : AuditTrailEntry
    {
        public ModifyTenant()
            : base()
        {
            Type = AuditTrailEntryType.ModifyTenant;
        }

        /// <summary>
        /// The new state of the <see cref="Tenant"/> being modified or created.
        /// </summary>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="Tenant"/> is a new record.
        /// </summary>
        public bool NewTenant { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { Tenant, NewTenant });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewTenant = jsonDoc.RootElement.GetProperty(nameof(NewTenant)).GetBoolean();
                Tenant = JsonSerializer.Deserialize<Tenant>(jsonDoc.RootElement.GetProperty(nameof(Tenant)).GetRawText());
            }
        }
    }
}

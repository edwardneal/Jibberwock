using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies a <see cref="Tier"/>.
    /// </summary>
    public class ModifyTier : AuditTrailEntry
    {
        public ModifyTier()
            : base()
        {
            Type = AuditTrailEntryType.ModifyTier;
        }

        /// <summary>
        /// The new state of the <see cref="Tier"/> being modified or created.
        /// </summary>
        public Tier Tier { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="Tier"/> is a new record.
        /// </summary>
        public bool NewTier { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { Tier, NewTier });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewTier = jsonDoc.RootElement.GetProperty(nameof(NewTier)).GetBoolean();
                Tier = JsonSerializer.Deserialize<Tier>(jsonDoc.RootElement.GetProperty(nameof(Tier)).GetRawText());
            }
        }
    }
}

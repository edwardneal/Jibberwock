using Jibberwock.DataModels.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry for a <see cref="Tenant"/>'s subscription.
    /// </summary>
    public class Subscription : AuditTrailEntry
    {
        public Subscription()
            : base()
        {
            Type = AuditTrailEntryType.Subscription;
        }

        /// <summary>
        /// If non-null, this is created <see cref="Jibberwock.DataModels.Products.Subscription"/>.
        /// </summary>
        public Jibberwock.DataModels.Products.Subscription ResultantSubscription { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { ResultantSubscription });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                ResultantSubscription = JsonSerializer.Deserialize<Jibberwock.DataModels.Products.Subscription>(jsonDoc.RootElement.GetProperty(nameof(ResultantSubscription)).GetRawText());
            }
        }
    }
}

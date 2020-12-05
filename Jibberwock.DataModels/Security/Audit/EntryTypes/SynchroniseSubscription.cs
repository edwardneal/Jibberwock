using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which synchronises a <see cref="Jibberwock.DataModels.Products.Subscription"/> with the billing provider.
    /// </summary>
    public class SynchroniseSubscription : AuditTrailEntry
    {
        public SynchroniseSubscription()
            : base()
        {
            Type = AuditTrailEntryType.SynchroniseSubscription;
        }

        /// <summary>
        /// The internal IDs of the subscriptions being linked to this billing provider's subscription.
        /// </summary>
        public IEnumerable<long> SubscriptionIds { get; set; }

        /// <summary>
        /// The status these subscriptions will be set to.
        /// </summary>
        public SubscriptionStatus? NewStatus { get; set; }

        /// <summary>
        /// The unique identifier of the subscription in the billing provider.
        /// </summary>
        public string ExternalSubscriptionIdentifier { get; set; }

        /// <summary>
        /// The unique identifier of the subscription's most recent invoice in the billing provider.
        /// </summary>
        public string LatestInvoiceExternalIdentifier { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { SubscriptionIds, NewStatus, ExternalSubscriptionIdentifier, LatestInvoiceExternalIdentifier });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                SubscriptionIds = jsonDoc.RootElement.GetProperty(nameof(SubscriptionIds))
                    .EnumerateArray()
                    .Select(x => x.GetInt64())
                    .ToArray();
                NewStatus = jsonDoc.RootElement.GetProperty(nameof(NewStatus)).ValueKind == JsonValueKind.Null
                    ? (SubscriptionStatus?)null
                    : (SubscriptionStatus)jsonDoc.RootElement.GetProperty(nameof(NewStatus)).GetInt32();
                ExternalSubscriptionIdentifier = jsonDoc.RootElement.GetProperty(nameof(ExternalSubscriptionIdentifier)).GetString();
                LatestInvoiceExternalIdentifier = jsonDoc.RootElement.GetProperty(nameof(LatestInvoiceExternalIdentifier)).GetString();
            }
        }
    }
}

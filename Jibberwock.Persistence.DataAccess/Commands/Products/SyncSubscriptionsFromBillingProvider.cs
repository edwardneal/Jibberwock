using Dapper;
using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.Commands.Auditing;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// Synchronises the details of a list of subscriptions from a billing provider.
    /// </summary>
    public class SyncSubscriptionsFromBillingProvider : AuditingCommand<bool, Jibberwock.DataModels.Security.Audit.EntryTypes.SynchroniseSubscription>
    {
        /// <summary>
        /// The list of subscriptions to link to a billing provider's subscriptions. Can be empty.
        /// </summary>
        [Required]
        public IEnumerable<Subscription> Subscriptions { get; set; }

        /// <summary>
        /// The new status of the subscriptions, or <c>null</c> to leave it unchanged.
        /// </summary>
        [Required]
        public SubscriptionStatus? Status { get; set; }

        /// <summary>
        /// The external identifier of the billing provider subscription. Required.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// The external identifier of the most recent invoice.
        /// </summary>
        public string LatestInvoiceExternalIdentifier { get; set; }

        public SyncSubscriptionsFromBillingProvider(ILogger logger, User performedBy, string connectionId, int serviceId, string comment,
            IEnumerable<Subscription> subscriptions, SubscriptionStatus? status, string externalIdentifier, string latestInvoiceExternalIdentifier)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Subscriptions = subscriptions;
            Status = status;
            ExternalIdentifier = externalIdentifier;
            LatestInvoiceExternalIdentifier = latestInvoiceExternalIdentifier;
        }

        protected override async Task<bool> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.SynchroniseSubscription provisionalAuditTrailEntry)
        {
            if (Subscriptions.Any(s => s.Id == 0))
                throw new ArgumentOutOfRangeException(nameof(Subscriptions), "Subscriptions.Id[] must always have values");

            var subscriptionIdsParameter = (from subs in Subscriptions
                                            select new TableTypes.Products.Subscription(subs.Id))
                                            .AsTableValuedParameter("tenants.udt_Subscription");
            var databaseConnection = await dataSource.GetDbConnection();

            await databaseConnection.ExecuteAsync("tenants.usp_SyncSubscriptionsFromBillingProvider",
                new
                {
                    Subscription_External_Identifier = ExternalIdentifier,
                    Status_ID = Status,
                    Last_Invoice_External_Identifier = LatestInvoiceExternalIdentifier,
                    Subscription_IDs = subscriptionIdsParameter
                }, transaction: transaction, commandType: CommandType.StoredProcedure, commandTimeout: 30);

            provisionalAuditTrailEntry.SubscriptionIds = Subscriptions.Select(s => s.Id);
            provisionalAuditTrailEntry.NewStatus = Status;
            provisionalAuditTrailEntry.ExternalSubscriptionIdentifier = ExternalIdentifier;
            provisionalAuditTrailEntry.LatestInvoiceExternalIdentifier = LatestInvoiceExternalIdentifier;

            return true;
        }
    }
}

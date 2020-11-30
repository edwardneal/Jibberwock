using Dapper;
using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Tenants;
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

namespace Jibberwock.Persistence.DataAccess.Commands.Tenants
{
    /// <summary>
    /// Subscribes a tenant to a product.
    /// </summary>
    public class Subscribe : AuditingCommand<Subscription, Jibberwock.DataModels.Security.Audit.EntryTypes.Subscription>
    {
        /// <summary>
        /// The subscription to be created.
        /// </summary>
        [Required]
        public Subscription Subscription { get; set; }

        public Subscribe(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, Subscription subscription)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Subscription = subscription;
        }

        protected override async Task<Subscription> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.Subscription provisionalAuditTrailEntry)
        {
            if (Subscription.Id != 0)
                throw new ArgumentOutOfRangeException(nameof(Subscription), "Subscription.Id must not have a value");
            if (Subscription.ProductTier == null)
                throw new ArgumentNullException(nameof(Subscription), "Subscription.ProductTier must have a value");
            if (Subscription.ProductTier.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Subscription), "Subscription.ProductTier.Id must have a value");
            if (Subscription.Configuration == null)
                throw new ArgumentNullException(nameof(Subscription), "Subscription.Configuration must have a value");
            if (string.IsNullOrWhiteSpace(Subscription.Configuration.ConfigurationString))
                throw new ArgumentNullException(nameof(Subscription), "Subscription.Configuration.ConfigurationString must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var subscriptions = await databaseConnection.QueryAsync<Subscription, Tenant, Tier, Subscription>("tenants.usp_CreateSubscription",
                (sub, ten, tier) =>
                {
                    if (ten != null && ten.Id != 0)
                        sub.Tenant = ten;
                    if (tier != null & tier.Id != 0)
                        sub.ProductTier = tier;

                    return sub;
                },
                new
                {
                    Tenant_ID = Subscription.Tenant.Id,
                    Tier_ID = Subscription.ProductTier.Id,
                    Product_Configuration = Subscription.Configuration.ConfigurationString,
                    Start_Date = Subscription.StartDate,
                    End_Date = Subscription.EndDate
                },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            var resultantSubscription = subscriptions.FirstOrDefault();

            Subscription = resultantSubscription;
            provisionalAuditTrailEntry.ResultantSubscription = resultantSubscription;
            provisionalAuditTrailEntry.RelatedTenant = resultantSubscription.Tenant;

            return resultantSubscription;
        }
    }
}

using Jibberwock.DataModels.Tenants;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.WebHooks.Stripe
{
    public static class JibberwockEventProcessing
    {
        public static async Task UpdateCustomer(IServiceProvider serviceProvider, Customer customer)
        {
            var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var webApiConfiguration = serviceProvider.GetRequiredService<IOptions<WebApiConfiguration>>()?.Value;
            var logger = loggerFactory.CreateLogger(string.Join('.', typeof(JibberwockEventProcessing).FullName, nameof(UpdateCustomer)));
            var sqlDataSource = serviceProvider.GetRequiredService<SqlServerDataSource>();

            var finalTenant = new Tenant()
            {
                Name = customer.Name,
                ExternalId = customer.Id,
                BillingContact = new Contact()
                {
                    EmailAddress = customer.Email,
                    TelephoneNumber = customer.Phone
                }
            };

            // Get the tenant ID if needed
            if (customer.Metadata.TryGetValue("jibberwock_id", out var rawTenantId)
                && long.TryParse(rawTenantId, out var tenantId))
            { finalTenant.Id = tenantId; }

            // Sync the Jibberwock tenant from the existing billing customer record
            var syncTenantCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.SyncTenantFromBillingProvider(logger, null, contextAccessor.HttpContext.TraceIdentifier, webApiConfiguration.Authorization.DefaultServiceId, null, finalTenant);

            await syncTenantCommand.Execute(sqlDataSource);
        }

        public static async Task MaintainSubscription(IServiceProvider serviceProvider, Subscription subscription)
        {
            var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var webApiConfiguration = serviceProvider.GetRequiredService<IOptions<WebApiConfiguration>>()?.Value;
            var logger = loggerFactory.CreateLogger(string.Join('.', typeof(JibberwockEventProcessing).FullName, nameof(MaintainSubscription)));
            var sqlDataSource = serviceProvider.GetRequiredService<SqlServerDataSource>();

            Jibberwock.DataModels.Products.SubscriptionStatus? desiredSubscriptionStatus = null;
            var subscriptions = new List<Jibberwock.DataModels.Products.Subscription>();

            switch (subscription?.Status?.ToLower())
            {
                case SubscriptionStatuses.Trialing:
                    desiredSubscriptionStatus = DataModels.Products.SubscriptionStatus.Trial;
                    break;
                case SubscriptionStatuses.Active:
                    desiredSubscriptionStatus = DataModels.Products.SubscriptionStatus.Active;
                    break;
                case SubscriptionStatuses.PastDue:
                    desiredSubscriptionStatus = DataModels.Products.SubscriptionStatus.Expired;
                    break;
                case SubscriptionStatuses.Canceled:
                case SubscriptionStatuses.Unpaid:
                case SubscriptionStatuses.IncompleteExpired:
                    desiredSubscriptionStatus = DataModels.Products.SubscriptionStatus.Unpaid;
                    break;
                case SubscriptionStatuses.Incomplete:
                    desiredSubscriptionStatus = DataModels.Products.SubscriptionStatus.PaymentPending;
                    break;
            }

            // Map the jibberwock_ids parameter to a list of Jibberwock subscription IDs, if it's present
            if (subscription.Metadata.TryGetValue("jibberwock_ids", out var rawSubscriptionIds))
            {
                var idList = rawSubscriptionIds.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach(var id in idList)
                {
                    if (long.TryParse(id, out var parsedId))
                    { subscriptions.Add(new Jibberwock.DataModels.Products.Subscription() { Id = parsedId }); }
                }
            }

            var syncSubscriptionCommand = new Jibberwock.Persistence.DataAccess.Commands.Products.SyncSubscriptionsFromBillingProvider(logger, null, contextAccessor.HttpContext.TraceIdentifier, webApiConfiguration.Authorization.DefaultServiceId, null,
                subscriptions, desiredSubscriptionStatus, subscription.Id, subscription.LatestInvoiceId);

            await syncSubscriptionCommand.Execute(sqlDataSource);
        }
    }
}

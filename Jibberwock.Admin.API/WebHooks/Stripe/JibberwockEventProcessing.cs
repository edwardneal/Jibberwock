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
            //  update Tenant.Name, Tenant.BillingContact.Email, Tenant.BillingContact.PhoneNumber
            //      if possible, use the Stripe ID. if not, check to see if metadata.jibberwock_id matches. if it does, update External_Identifier to match.
            customer.ToString();
        }

        public static async Task MaintainSubscription(IServiceProvider serviceProvider, Subscription subscription)
        {
            // customer.subscription.created:
            //  update Subscription.External_Identifiers based upon metadata.jibberwock_ids.split(";"), setting all records to the subscription ID
            //      this builds the cross-reference
            // customer.subscription.created, customer.subscription.updated:
            //  set Status_ID based upon status:
            //      incomplete = 1
            //      active = 3
            //      trialing = 2
            //      past_due = 4
            //      canceled = 6
            //      unpaid = 6
            //  set Last_Invoice_External_Identifier based on "latest_invoice"
            subscription.ToString();
        }
    }
}

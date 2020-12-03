using Jibberwock.Persistence.DataAccess.DataSources.Payments;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Payments.Stripe
{
    internal class StripePaymentProvider : IPaymentProvider, ICustomerDataSource, IPaymentSessionFactory
    {
        private readonly Configuration.StripeConfiguration _stripeConfiguration;

        ICustomerDataSource IPaymentProvider.CustomerDataSource { get => this; }

        IPaymentSessionFactory IPaymentProvider.PaymentSessionFactory { get => this; }

        public StripePaymentProvider(IOptions<Configuration.WebApiConfiguration> webApiConfiguration)
        {
            StripeConfiguration.ApiKey = webApiConfiguration?.Value?.Stripe?.SecretApiKey;
            _stripeConfiguration = webApiConfiguration?.Value.Stripe;
        }

        async Task<string> ICustomerDataSource.CreateCustomer(string name, string emailAddress, string phoneNumber, Dictionary<string, string> metadata)
        {
            var custService = new CustomerService();
            var creationOptions = new CustomerCreateOptions()
            {
                Name = name,
                Email = emailAddress,
                Phone = phoneNumber,
                Metadata = metadata
            };

            var customer = await custService.CreateAsync(creationOptions);

            return customer.Id;
        }

        async Task ICustomerDataSource.UpdateCustomer(string id, string name, string emailAddress, string phoneNumber, Dictionary<string, string> metadata)
        {
            var custService = new CustomerService();
            var updateOptions = new CustomerUpdateOptions()
            {
                Name = name,
                Email = emailAddress,
                Phone = phoneNumber,
                Metadata = metadata
            };

            await custService.UpdateAsync(id, updateOptions);
        }

        async Task<string> IPaymentSessionFactory.CreateSubscriptionSession(string returnUrlBase, string customerId, IEnumerable<string> productIds, Dictionary<string, string> subscriptionMetadata)
        {
            var sessService = new SessionService();
            var sessionCreationOptions = new SessionCreateOptions()
            {
                Customer = customerId,
                Mode = "subscription",
                LineItems = (from prod in productIds
                             select new SessionLineItemOptions()
                             {
                                 Price = prod,
                                 Quantity = 1
                             }).ToList(),
                SubscriptionData = new SessionSubscriptionDataOptions()
                {
                    Metadata = subscriptionMetadata
                },
                PaymentMethodTypes = new List<string>() { "card" },
                SuccessUrl = returnUrlBase + "?result=success&session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = returnUrlBase + "?result=cancel"
            };

            var session = await sessService.CreateAsync(sessionCreationOptions);

            return session.Id;
        }
    }
}

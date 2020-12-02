﻿using Jibberwock.Core.API.ActionModels.Invitations;
using Jibberwock.Core.API.ActionModels.Tenants;
using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Products.Configuration;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.Controllers.Tenants
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "EasyAuth")]
    [Route("api/[controller]")]
    public class TenantController : JibberwockControllerBase
    {
        private readonly IQueueDataSource _queueDataSource;
        private readonly Jibberwock.Shared.Payments.IPaymentProvider _paymentProvider;

        public TenantController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever, IQueueDataSource queueDataSource,
            Jibberwock.Shared.Payments.IPaymentProvider paymentProvider)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        {
            _queueDataSource = queueDataSource;
            _paymentProvider = paymentProvider;
        }

        /// <summary>
        /// Gets all <see cref="Tenant"/>s which the current <see cref="User"/> has access to.
        /// </summary>
        /// <response code="200" nullable="false">An array of <see cref="Tenant"/>s accessible by the current <see cref="User"/>.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Tenant>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccessibleTenants()
        {
            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            // Get every tenant associated with the user
            var getTenantsCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.GetTenantsByUser(Logger, currUser, true);
            var allTenants = await getTenantsCommand.Execute(SqlServerDataSource);

            return Ok(allTenants);
        }

        /// <summary>
        /// Create a <see cref="Tenant"/>.
        /// </summary>
        /// <param name="creationOptions">The information needed to create the <see cref="Tenant"/>.</param>
        /// <response code="201" nullable="false">A <see cref="CreateTenantResponse" /> containing information about the created <see cref="Tenant"/> and any next steps for payment.</response>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(CreateTenantResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTenant([FromBody] TenantCreationOptions creationOptions)
        {
            // Filter out the worst possible results: completely missing fields
            // Set up some sane defaults for Invitations and Subscriptions - if those are null, they're probably empty
            if (creationOptions == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }
            if (string.IsNullOrWhiteSpace(creationOptions.Name))
            { ModelState.AddModelError(ErrorResponses.MissingTenantName, string.Empty); }

            if (creationOptions.Contact == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, "contact"); }
            else
            {
                // Before performing any validation, set the contact details appropriately
                if (creationOptions.Contact.UseOwnDetails)
                {
                    // Set the person's name and email address ourselves
                    creationOptions.Contact.Name = User.FindFirst("name")?.Value;
                    creationOptions.Contact.EmailAddress = User.FindFirst("emails")?.Value;
                }
                // We always need to have a contact name, and either an email address or phone number
                if (string.IsNullOrWhiteSpace(creationOptions.Contact.Name))
                { ModelState.AddModelError(ErrorResponses.MissingContactName, string.Empty); }
                if (string.IsNullOrWhiteSpace(creationOptions.Contact.EmailAddress) && string.IsNullOrWhiteSpace(creationOptions.Contact.PhoneNumber))
                { ModelState.AddModelError(ErrorResponses.MissingContactEmailAddress, string.Empty); }
            }

            if (creationOptions.Invitations == null)
            { creationOptions.Invitations = Enumerable.Empty<InvitationRequest>(); }
            for (int i = 0; i < creationOptions.Invitations.Count(); i++)
            {
                var invitation = creationOptions.Invitations.ToArray()[i];

                if (invitation == null)
                { ModelState.AddModelError(ErrorResponses.MissingBody, $"invitations[{i}]"); }
                else
                {
                    if (string.IsNullOrWhiteSpace(invitation.EmailAddress))
                    { ModelState.AddModelError(ErrorResponses.MissingInvitationEmailAddress, $"invitations[{i}]"); }
                    if (string.IsNullOrWhiteSpace(invitation.IdentityProvider))
                    { ModelState.AddModelError(ErrorResponses.MissingInvitationIdentityProvider, $"invitations[{i}]"); }
                }
            }

            if (creationOptions.Subscriptions == null)
            { creationOptions.Subscriptions = Enumerable.Empty<TenantSubscription>(); }
            for (int i = 0; i < creationOptions.Subscriptions.Count(); i++)
            {
                var subscription = creationOptions.Subscriptions.ToArray()[i];

                if (subscription == null)
                { ModelState.AddModelError(ErrorResponses.MissingBody, $"subscriptions[{i}]"); }
            }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            // Create the core tenant (including contact, groups, ACLs)...
            var tenant = new Tenant()
            {
                Name = creationOptions.Name,
                BillingContact = new Contact()
                {
                    FullName = creationOptions.Contact.Name,
                    EmailAddress = creationOptions.Contact.EmailAddress,
                    TelephoneNumber = creationOptions.Contact.PhoneNumber
                }
            };
            var createTenantCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.CreateTenant(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                tenant);
            var createdTenant = await createTenantCommand.Execute(SqlServerDataSource);
            var creationResponse = new CreateTenantResponse() { TenantId = tenant.Id };

            // ...invite users to this tenant, sending emails as needed...
            foreach (var invitation in creationOptions.Invitations)
            {
                var inviteUserCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.Invite(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                    new Invitation() { EmailAddress = invitation.EmailAddress, ExternalIdentityProvider = invitation.IdentityProvider, Tenant = createdTenant.Result }, invitation.SendEmail,
                    _queueDataSource, WebApiConfiguration.ServiceBus.Queues.Notifications, invitation.LoginRedirectUrl);

                await inviteUserCommand.Execute(SqlServerDataSource);
            }

            var subscriptionList = new List<Subscription>();

            // ...add product subscriptions...
            foreach (var subsRequest in creationOptions.Subscriptions)
            {
                var subscribeCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.Subscribe(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                    new Subscription() { Tenant = tenant, ProductTier = new Tier() { Id = subsRequest.ProductTierId }, StartDate = DateTimeOffset.Now, Configuration = new RawProductConfiguration(subsRequest.ProductConfiguration) });
                var subscription = await subscribeCommand.Execute(SqlServerDataSource);

                subscriptionList.Add(subscription.Result);
            }

            var paidSubscriptions = subscriptionList.Where(s => !string.IsNullOrWhiteSpace(s.ProductTier.ExternalId));

            // ...and set up a Stripe session if necessary
            if (paidSubscriptions.Any())
            {
                // Make sure we create an external customer first, updating the ExternalId property to match
                var createExternalCustomerCommand = new Jibberwock.Persistence.DataAccess.Commands.Payments.CreateExternalCustomer(Logger, tenant, _paymentProvider.CustomerDataSource);

                tenant = await createExternalCustomerCommand.Execute(SqlServerDataSource);

                // Set up the initial properties: the return URLs and the metadata
                var resultantReturnUrl = creationOptions.PaymentUrlBase.Replace("{tenantId}", tenant.Id.ToString(), StringComparison.OrdinalIgnoreCase);
                var joinedSubscriptionIds = string.Join(";", paidSubscriptions.Select(s => s.Id.ToString()));
                var subscriptionMetadata = new Dictionary<string, string>()
                { { "jibberwock_ids", joinedSubscriptionIds } };

                // Create the session in Stripe.
                // This means that a Stripe subscription will be a bundle of Jibberwock subscriptions which were bought at the same time.
                creationResponse.StripeSessionId = await _paymentProvider.PaymentSessionFactory.CreateSubscriptionSession(resultantReturnUrl, tenant.ExternalId, paidSubscriptions.Select(s => s.ProductTier.ExternalId), subscriptionMetadata);
                creationResponse.StripePublishableKey = WebApiConfiguration.Stripe.PublishableApiKey;
                creationResponse.PaymentRequired = true;
            }

            // NB: Stripe will trigger webhooks in the administrator site. These webhooks will be triggered when somebody pays, and they'll call tenants.usp_StartPaidSubscription.
            // To support this, we need to pass the subscription ID as metadata to the Stripe session.
            return Ok(creationResponse);
        }
    }
}

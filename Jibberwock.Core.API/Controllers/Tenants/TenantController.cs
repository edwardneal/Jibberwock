﻿using Jibberwock.Core.API.ActionModels.Invitations;
using Jibberwock.Core.API.ActionModels.Tenants;
using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Products.Configuration;
using Jibberwock.DataModels.Security;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Authorization;
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

        /// <summary>
        /// Gets a <see cref="Tenant"/> by its ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <remarks>This requires <see cref="Permission.Read"/> over the <see cref="Tenant"/>.</remarks>
        /// <response code="200" nullable="false">A <see cref="Tenant"/> which is accessible by the current <see cref="User"/>.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Tenant), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([ResourcePermissions(SecurableResourceType.Tenant, Permission.Read)] long id)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var getTenantCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.GetTenantById(Logger, id);
            var tenant = await getTenantCommand.Execute(SqlServerDataSource);

            if (tenant != null)
            { return Ok(tenant); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Gets all groups in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <remarks>This requires <see cref="Permission.Read"/> over the <see cref="Tenant"/>. It only retrieves top-level information about each <see cref="Group"/>.</remarks>
        /// <response code="200" nullable="false">A set of <see cref="Group"/> items, one for every group in the specified <see cref="Tenant"/>.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Group>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTenantSecurityGroups([ResourcePermissions(SecurableResourceType.Tenant, Permission.Read)] long id)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var listGroupsCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.ListTenantGroups(Logger, new Tenant() { Id = id });
            var groups = await listGroupsCommand.Execute(SqlServerDataSource);

            if (groups != null && groups.Any())
            { return Ok(groups); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Creates a single group in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="group">The <see cref="Group"/> to create.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>. It retrieves all information for the specified <see cref="Group"/>.</remarks>
        /// <response code="200" nullable="false">A single <see cref="Group"/>, with all fields populated.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups")]
        [HttpPost]
        [ProducesResponseType(typeof(Group), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSingleTenantSecurityGroup([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, [FromBody] Group group)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (group == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }
            if (string.IsNullOrWhiteSpace(group.Name))
            { ModelState.AddModelError(ErrorResponses.MissingGroupName, string.Empty); }

            if (group.Users != null)
            {
                var memberships = group.Users.ToArray();

                for (int i = 0; i < memberships.Length; i++)
                {
                    var groupMembership = memberships[i];

                    if (groupMembership == null)
                    { ModelState.AddModelError(ErrorResponses.MissingBody, $"users[{i}]"); }
                    if (groupMembership.User == null)
                    { ModelState.AddModelError(ErrorResponses.MissingBody, $"users[{i}].user"); }
                    if (groupMembership.User.Id == 0)
                    { ModelState.AddModelError(ErrorResponses.InvalidId, $"users[{i}].user.id"); }
                }
            }

            if (group.AccessControlEntries != null)
            {
                var accessControlEntries = group.AccessControlEntries.ToArray();

                for(int i = 0; i < accessControlEntries.Length; i++)
                {
                    var accessControlEntry = accessControlEntries[i];

                    if (accessControlEntry == null)
                    { ModelState.AddModelError(ErrorResponses.MissingBody, $"accessControlEntries[{i}]"); }
                    if (accessControlEntry.Resource == null)
                    { ModelState.AddModelError(ErrorResponses.MissingBody, $"accessControlEntries[{i}].resource"); }
                    if (accessControlEntry.Resource.Id == 0)
                    { ModelState.AddModelError(ErrorResponses.InvalidId, $"accessControlEntries[{i}].resource.id"); }
                }
            }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var groupToCreate = new Group()
            {
                Name = group.Name,
                Users = group.Users?.Select(gm => new GroupMembership()
                {
                    User = new User() { Id = gm.User.Id },
                    Enabled = gm.Enabled
                }).ToArray(),
                AccessControlEntries = group.AccessControlEntries?.Select(ace => new AccessControlEntry()
                {
                    Resource = ace.Resource,
                    Permission = ace.Permission
                }),
                Tenant = new Tenant() { Id = id }
            };

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var createGroupCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.CreateSecurityGroup(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, groupToCreate);
            var createdGroup = await createGroupCommand.Execute(SqlServerDataSource);

            if (createdGroup?.Result != null)
            { return Ok(createdGroup.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Gets the details of a single group in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <remarks>This requires <see cref="Permission.Read"/> over the <see cref="Tenant"/>. It retrieves all information for the specified <see cref="Group"/>.</remarks>
        /// <response code="200" nullable="false">A single <see cref="Group"/>, with all fields populated.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> or the <paramref name="groupId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSingleTenantSecurityGroup([ResourcePermissions(SecurableResourceType.Tenant, Permission.Read)]  long id, long groupId)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var getSecurityGroupCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetSecurityGroupById(Logger, new Group() { Id = groupId, Tenant = new Tenant() { Id = id } });
            var retrievedGroup = await getSecurityGroupCommand.Execute(SqlServerDataSource);

            if (retrievedGroup != null)
            { return Ok(retrievedGroup); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Updates a single group in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <param name="group">The information needed to create the <see cref="Group"/>.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>. It retrieves top-level information for the specified <see cref="Group"/>.</remarks>
        /// <response code="200" nullable="false">A single <see cref="Group"/>, with top-level fields populated.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> or the <paramref name="groupId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}")]
        [HttpPut]
        [ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSingleTenantSecurityGroup([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, long groupId, [FromBody] Group group)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }
            if (group == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }
            if (string.IsNullOrWhiteSpace(group.Name))
            { ModelState.AddModelError(ErrorResponses.MissingGroupName, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var updateSecurityGroupCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.UpdateSecurityGroup(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new Group() { Id = groupId, Name = group.Name, Tenant = new Tenant() { Id = id } });
            var updatedGroup = await updateSecurityGroupCommand.Execute(SqlServerDataSource);

            if (updatedGroup?.Result != null)
            { return Ok(updatedGroup.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Removes a single group from this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>.</remarks>
        /// <response code="200" nullable="false">This <see cref="Group"/> has been deleted.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> or <paramref name="groupId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}")]
        [HttpDelete]
        [ProducesResponseType(typeof(GroupMembership), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSingleTenantSecurityGroupMembership([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, long groupId)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var deleteGroupCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.DeleteSecurityGroup(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new Group() { Id = groupId, Tenant = new Tenant() { Id = id } });

            await deleteGroupCommand.Execute(SqlServerDataSource);

            return Ok();
        }

        /// <summary>
        /// Adds a user to a specific group in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <param name="groupMembership">The information needed to add a member to the <see cref="Group"/>.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>. It retrieves top-level information for the specified <see cref="Group"/>.</remarks>
        /// <response code="200" nullable="false">A single <see cref="GroupMembership"/>, with top-level fields populated.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> or the <paramref name="groupId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}/members")]
        [HttpPost]
        [ProducesResponseType(typeof(GroupMembership), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSingleTenantSecurityGroupMembership([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, long groupId, [FromBody] GroupMembership groupMembership)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }
            if (groupMembership == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }
            if (groupMembership.User == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, "group"); }
            if (groupMembership.User.Id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, "user.id"); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var createMembershipCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.CreateSecurityGroupMembership(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new GroupMembership() { Group = new Group() { Id = groupId, Tenant = new Tenant() { Id = id } }, Enabled = groupMembership.Enabled, User = new User() { Id = groupMembership.User.Id } });
            var newMembership = await createMembershipCommand.Execute(SqlServerDataSource);

            if (newMembership?.Result != null)
            { return Ok(newMembership.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Enables or disables a user's group membership in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <param name="groupMembershipId">The ID of the <see cref="GroupMembership"/>.</param>
        /// <param name="groupMembership">The information needed to add a member to the <see cref="Group"/>.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>. It retrieves top-level information for the specified <see cref="Group"/>.</remarks>
        /// <response code="200" nullable="false">A single <see cref="GroupMembership"/>, with the <see cref="GroupMembership.Enabled"/> field populated.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/>, <paramref name="groupId"/> or <paramref name="groupMembershipId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}/members/{groupMembershipId:int}")]
        [HttpPut]
        [ProducesResponseType(typeof(GroupMembership), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSingleTenantSecurityGroupMembership([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, long groupId, long groupMembershipId, [FromBody] GroupMembership groupMembership)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }
            if (groupMembershipId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupMembershipId)); }
            if (groupMembership == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var updateMembershipCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.UpdateSecurityGroupMembership(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new GroupMembership() { Id = groupMembershipId, Group = new Group() { Id = groupId, Tenant = new Tenant() { Id = id } }, Enabled = groupMembership.Enabled });
            var updatedMembership = await updateMembershipCommand.Execute(SqlServerDataSource);

            if (updatedMembership?.Result != null)
            { return Ok(updatedMembership.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Removes a user's group membership in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <param name="groupMembershipId">The ID of the <see cref="GroupMembership"/>.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>.</remarks>
        /// <response code="200" nullable="false">This <see cref="GroupMembership"/> has been deleted.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/>, <paramref name="groupId"/> or <paramref name="groupMembershipId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}/members/{groupMembershipId:int}")]
        [HttpDelete]
        [ProducesResponseType(typeof(GroupMembership), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSingleTenantSecurityGroupMembership([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, long groupId, long groupMembershipId)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }
            if (groupMembershipId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupMembershipId)); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var deleteMembershipCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.DeleteSecurityGroupMembership(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new GroupMembership() { Id = groupMembershipId, Group = new Group() { Id = groupId, Tenant = new Tenant() { Id = id } } });
            
            await deleteMembershipCommand.Execute(SqlServerDataSource);

            return Ok();
        }

        /// <summary>
        /// Adds an access control entry which grants permissions to a specific group in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <param name="accessControlEntry">The information needed to add an access control entry to the <see cref="Group"/>.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>. It retrieves top-level information for the specified <see cref="AccessControlEntry"/>.</remarks>
        /// <response code="200" nullable="false">A single <see cref="AccessControlEntry"/>.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> or the <paramref name="groupId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}/permissions")]
        [HttpPost]
        [ProducesResponseType(typeof(AccessControlEntry), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSingleAccessControlEntry([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, long groupId, [FromBody] AccessControlEntry accessControlEntry)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }
            if (accessControlEntry == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }
            if (accessControlEntry.Resource == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, "resource"); }
            if (accessControlEntry.Resource.Id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, "resource.id"); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var createAccessControlEntryCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.CreateAccessControlEntry(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new AccessControlEntry() { Group = new Group() { Id = groupId, Tenant = new Tenant() { Id = id } }, Resource = accessControlEntry.Resource, Permission = accessControlEntry.Permission });
            var newAccessControlEntry = await createAccessControlEntryCommand.Execute(SqlServerDataSource);

            if (newAccessControlEntry?.Result != null)
            { return Ok(newAccessControlEntry.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Removes an access control entry from a specific group in this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="groupId">The ID of the <see cref="Group"/>.</param>
        /// <param name="accessControlEntryId">The ID of the <see cref="AccessControlEntry"/>.</param>
        /// <remarks>This requires <see cref="Permission.Change"/> over the <see cref="Tenant"/>.</remarks>
        /// <response code="200" nullable="false">This <see cref="AccessControlEntry"/> has been deleted.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/>, <paramref name="groupId"/> or <paramref name="accessControlEntryId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/groups/{groupId:int}/permissions/{accessControlEntryId:int}")]
        [HttpDelete]
        [ProducesResponseType(typeof(GroupMembership), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSingleAccessControlEntry([ResourcePermissions(SecurableResourceType.Tenant, Permission.Change)] long id, long groupId, long accessControlEntryId)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (groupId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(groupId)); }
            if (accessControlEntryId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(accessControlEntryId)); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var deleteAccessControlEntryCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.DeleteAccessControlEntry(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new AccessControlEntry() { Id = accessControlEntryId, Group = new Group() { Id = groupId, Tenant = new Tenant() { Id = id } } });

            await deleteAccessControlEntryCommand.Execute(SqlServerDataSource);

            return Ok();
        }

        /// <summary>
        /// Gets all <see cref="SecurableResource"/>s in this <see cref="Tenant"/> with a name which matches <paramref name="filter"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="filter">The filter to apply to various <see cref="SecurableResource"/> names.</param>
        /// <remarks>This requires <see cref="Permission.Read"/> over the <see cref="Tenant"/>. It only returns sub-tenant <see cref="SecurableResource"/>s which the user has some kind of access to.</remarks>
        /// <response code="200" nullable="false">A set of <see cref="SecurableResource"/> derivatives.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/securableresources")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SecurableResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSecurableResourcesByFilter([ResourcePermissions(SecurableResourceType.Tenant, Permission.Read)] long id, [FromQuery] string filter)
        {
            var safeFilter = filter;

            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }

            if (string.IsNullOrWhiteSpace(safeFilter))
            { safeFilter = "%"; }
            else
            { safeFilter = safeFilter.Replace("*", "%"); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var getResourcesCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetSecurableResourcesByName(Logger, safeFilter, currUser, new Tenant() { Id = id });
            var resources = await getResourcesCommand.Execute(SqlServerDataSource);

            return Ok(resources);
        }

        /// <summary>
        /// Gets all members of this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <remarks>This requires <see cref="Permission.Read"/> over the <see cref="Tenant"/>..</remarks>
        /// <response code="200" nullable="false">A set of <see cref="GroupMembership"/> instances.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/members")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GroupMembership>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTenantMembers([ResourcePermissions(SecurableResourceType.Tenant, Permission.Read)]  long id)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var getMembersGroupCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetWellKnownTenantSecurityGroup(Logger, new Tenant() { Id = id }, WellKnownGroupType.TenantMembers);
            var membersGroup = await getMembersGroupCommand.Execute(SqlServerDataSource);

            return Ok(membersGroup.Users);
        }

        /// <summary>
        /// Gets all invitations to this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <remarks>This requires <see cref="Permission.Read"/> over the <see cref="Tenant"/>..</remarks>
        /// <response code="200" nullable="false">A set of <see cref="Invitation"/> instances.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/invitations")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Invitation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTenantInvitations([ResourcePermissions(SecurableResourceType.Tenant, Permission.Read)] long id)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var getInvitationsCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.GetInvitationsByTenant(Logger, new Tenant() { Id = id }, false);
            var invitations = await getInvitationsCommand.Execute(SqlServerDataSource);

            return Ok(invitations);
        }

        /// <summary>
        /// Invites a single user account to this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="invitation">The information needed to invite somebody to the <see cref="Tenant"/>.</param>
        /// <remarks>This requires <see cref="Permission.Invite"/> over the <see cref="Tenant"/>. It retrieves top-level information for the specified <see cref="Invitation"/>.</remarks>
        /// <response code="200" nullable="false">A single <see cref="Invitation"/>.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/invitations")]
        [HttpPost]
        [ProducesResponseType(typeof(AccessControlEntry), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InviteSingleUser([ResourcePermissions(SecurableResourceType.Tenant, Permission.Invite)] long id, [FromBody] InvitationRequest invitation)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }

            if (invitation == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }
            else
            {
                if (string.IsNullOrWhiteSpace(invitation.EmailAddress))
                { ModelState.AddModelError(ErrorResponses.MissingInvitationEmailAddress, string.Empty); }
                if (string.IsNullOrWhiteSpace(invitation.IdentityProvider))
                { ModelState.AddModelError(ErrorResponses.MissingInvitationIdentityProvider, string.Empty); }
            }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var inviteUserCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.Invite(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                    new Invitation() { EmailAddress = invitation.EmailAddress, ExternalIdentityProvider = invitation.IdentityProvider, Tenant = new Tenant() { Id = id } }, invitation.SendEmail,
                    _queueDataSource, WebApiConfiguration.ServiceBus.Queues.Notifications, invitation.LoginRedirectUrl);

            var auditedInvitation = await inviteUserCommand.Execute(SqlServerDataSource);

            if (auditedInvitation?.Result == null)
            { return NotFound(); }
            else
            { return Ok(auditedInvitation.Result); }
        }

        /// <summary>
        /// Revokes a single invitation to this <see cref="Tenant"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Tenant"/>.</param>
        /// <param name="invitationId">The ID of the <see cref="Invitation"/>.</param>
        /// <remarks>This requires <see cref="Permission.Invite"/> over the <see cref="Tenant"/>.</remarks>
        /// <response code="200" nullable="false">This <see cref="Invitation"/> has been revoked.</response>
        /// <response code="400" nullable="false">The <paramref name="id"/> or <paramref name="invitationId"/> parameter was <c>0</c>.</response>
        /// <response code="401" nullable="false">The <see cref="Tenant"/> is not accessible by the current <see cref="User"/> or does not exist.</response>
        [Route("{id:int}/invitations/{invitationId:int}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeSingleInvitation([ResourcePermissions(SecurableResourceType.Tenant, Permission.Invite)] long id, long invitationId)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(id)); }
            if (invitationId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, nameof(invitationId)); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var revokeInvitationCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.RevokeInvitation(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, string.Empty,
                new Invitation() { Id = invitationId, Tenant = new Tenant() { Id = id } });

            await revokeInvitationCommand.Execute(SqlServerDataSource);

            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Shared.Http.Authorization;
using Jibberwock.DataModels.Security;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Jibberwock.Shared.Configuration;
using Jibberwock.Admin.API.ActionModels.Security;
using Jibberwock.DataModels.Users;
using Jibberwock.Shared.Http;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Admin.API.ActionModels.Notifications;
using Jibberwock.DataModels.Core;

namespace Jibberwock.Admin.API.Controllers.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : JibberwockControllerBase
    {
        private readonly IQueueDataSource _queueDataSource;

        public UserController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever, IQueueDataSource queueDataSource)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        {
            _queueDataSource = queueDataSource;
        }

        /// <summary>
        /// Gets all users with a name matching a pattern.
        /// </summary>
        /// <param name="name">The pattern to match names against.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="User"/> objects.</response>
        /// <response code="400" nullable="false">Unable to perform the search, see response for details.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetUsersByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            { ModelState.AddModelError(ErrorResponses.InvalidFilter, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var getUsersCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetUsersByName(Logger, name);

            return Ok(await getUsersCommand.Execute(SqlServerDataSource));
        }

        /// <summary>
        /// Gets a specific <see cref="User"/> by its ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="User"/> to retrieve.</param>
        /// <response code="200" nullable="false">The retrieved <see cref="User"/> object.</response>
        /// <response code="400" nullable="false">Unable to get the <see cref="User"/>, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="User"/> with the provided ID does not exist.</response>
        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetUserById([FromRoute] long id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getSingleUserCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetUserById(Logger, id);
            var singleUser = await getSingleUserCommand.Execute(SqlServerDataSource);

            if (singleUser == null)
            { return NotFound(); }
            else
            { return Ok(singleUser); }
        }

        /// <summary>
        /// Enables or disables a specific <see cref="User"/> by its ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="User"/> to enable or disable.</param>
        /// <param name="accessChangeSetting">The <see cref="User"/> properties to change.</param>
        /// <response code="200" nullable="false">The updated <see cref="User"/> object.</response>
        /// <response code="400" nullable="false">Unable to update the user, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="User"/> with the provided ID does not exist.</response>
        [Route("{id:int}")]
        [HttpPut]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> ControlUserAccess([FromRoute] long id, [FromBody, Bind(nameof(UserAccessChangeSetting.Enabled))] UserAccessChangeSetting accessChangeSetting)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (accessChangeSetting == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }
            
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var requestedState = new User() { Id = id, Enabled = accessChangeSetting.Enabled };
            var controlUserAccessCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.ControlUserAccess(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null, requestedState);
            
            var controlSuccessful = await controlUserAccessCommand.Execute(SqlServerDataSource);

            if (controlSuccessful.Result)
            { return Ok(requestedState); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Gets all currently-active <see cref="Notification"/>s for a specific <see cref="User"/> by its ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="User"/> to get the <see cref="Notification"/>s of.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="Notification"/> objects.</response>
        /// <response code="400" nullable="false">Unable to get the set of <see cref="Notification"/>s, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="User"/> with the provided ID does not exist.</response>
        [Route("{id:int}/notifications")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetNotifications([FromRoute] long id)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var listNotificationsCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.ListNotifications(Logger, new DataModels.Users.User() { Id = id });
            var notifications = await listNotificationsCommand.Execute(SqlServerDataSource);

            if (notifications == null)
            { return NotFound(); }
            else
            { return Ok(notifications); }
        }

        /// <summary>
        /// Notifies a specific <see cref="User"/> by its ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="User"/> to notify.</param>
        /// <param name="notification">Details of the <see cref="Notification"/> to send to this user.</param>
        /// <response code="200" nullable="false">The created <see cref="Notification"/> object.</response>
        /// <response code="400" nullable="false">Unable to notify this <see cref="User"/>, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="User"/> with the provided ID does not exist.</response>
        [Route("{id:int}/notifications")]
        [HttpPost]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> NotifyUser([FromRoute] long id, [FromBody] NotifyRequest notification)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (notification == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var requestedNotification = new Notification()
            {
                TargetUser = new User() { Id = id },
                Status =  notification.Status,
                Type = notification.Type,
                Priority = notification.Priority,
                StartDate = notification.StartDate,
                EndDate = notification.EndDate,
                Subject = notification.Subject,
                Message = notification.Message,
                AllowDismissal = notification.AllowDismissal
            };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var notifyCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.Notify(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                requestedNotification, notification.SendAsEmail, _queueDataSource, WebApiConfiguration.ServiceBus.Queues.Notifications);
            var resultantCommand = await notifyCommand.Execute(SqlServerDataSource);

            if (resultantCommand.Result != null)
            { return Ok(resultantCommand.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Updates a specific <see cref="Notification"/> for a specific <see cref="User"/> by their IDs.
        /// </summary>
        /// <param name="id">The ID of the <see cref="User"/> to update a <see cref="Notification"/> of.</param>
        /// <param name="notificationId">The ID of the <see cref="Notification"/> to update.</param>
        /// <param name="notification">Details of the <see cref="Notification"/> to update.</param>
        /// <response code="200" nullable="false">The updated <see cref="Notification"/> object.</response>
        /// <response code="400" nullable="false">Unable to update the <see cref="Notification"/> for this <see cref="User"/>, see response for details.</response>
        /// <response code="404" nullable="false">A <see cref="User"/> with the provided ID does not exist, or a <see cref="Notification"/> with the provided ID does not exist.</response>
        [Route("{id:int}/notifications/{notificationId:int}")]
        [HttpPut]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateNotification([FromRoute] long id, [FromRoute] long notificationId, [FromBody] NotifyRequest notification)
        {
            if (id == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (notificationId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (notification == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var requestedNotification = new Notification()
            {
                Id = notificationId,
                TargetUser = new User() { Id = id },
                Status = notification.Status,
                Type = notification.Type,
                Priority = notification.Priority,
                StartDate = notification.StartDate,
                EndDate = notification.EndDate,
                Subject = notification.Subject,
                Message = notification.Message,
                AllowDismissal = notification.AllowDismissal
            };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var updateNotificationCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.UpdateNotification(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                requestedNotification, notification.SendAsEmail, _queueDataSource, WebApiConfiguration.ServiceBus.Queues.Notifications);
            var resultantCommand = await updateNotificationCommand.Execute(SqlServerDataSource);

            if (resultantCommand.Result != null)
            { return Ok(resultantCommand.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Gets all currently-active <see cref="Notification"/>s for all <see cref="User"/>s.
        /// </summary>
        /// <response code="200" nullable="false">The retrieved set of <see cref="Notification"/> objects.</response>
        /// <response code="400" nullable="false">Unable to get the set of <see cref="Notification"/>s, see response for details.</response>
        [Route("all/notifications")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetGlobalUserNotifications()
        {
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var listNotificationsCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.ListNotifications(Logger);
            var notifications = await listNotificationsCommand.Execute(SqlServerDataSource);

            return Ok(notifications);
        }

        /// <summary>
        /// Notifies all <see cref="User"/>s.
        /// </summary>
        /// <param name="notification">Details of the <see cref="Notification"/> to send to all <see cref="User"/>s.</param>
        /// <response code="200" nullable="false">The created <see cref="Notification"/> object.</response>
        /// <response code="400" nullable="false">Unable to notify all <see cref="User"/>s, see response for details.</response>
        [Route("all/notifications")]
        [HttpPost]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> NotifyAllUsers([FromBody] NotifyRequest notification)
        {
            if (notification == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var requestedNotification = new Notification()
            {
                Status = notification.Status,
                Type = notification.Type,
                Priority = notification.Priority,
                StartDate = notification.StartDate,
                EndDate = notification.EndDate,
                Subject = notification.Subject,
                Message = notification.Message,
                AllowDismissal = notification.AllowDismissal
            };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var notifyCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.Notify(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                requestedNotification, notification.SendAsEmail, _queueDataSource, WebApiConfiguration.ServiceBus.Queues.Notifications);
            var resultantCommand = await notifyCommand.Execute(SqlServerDataSource);

            return Ok(resultantCommand.Result);
        }

        /// <summary>
        /// Updates a specific <see cref="Notification"/> for all <see cref="User"/>s.
        /// </summary>
        /// <param name="notificationId">The ID of the <see cref="Notification"/> to update.</param>
        /// <param name="notification">Details of the <see cref="Notification"/> to update.</param>
        /// <response code="200" nullable="false">The updated <see cref="Notification"/> object.</response>
        /// <response code="400" nullable="false">Unable to update the <see cref="Notification"/> for all <see cref="User"/>s, see response for details.</response>
        /// <response code="404" nullable="false">A global <see cref="Notification"/> with the provided ID does not exist.</response>
        [Route("all/notifications/{notificationId:int}")]
        [HttpPut]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateGlobalNotification([FromRoute] long notificationId, [FromBody] NotifyRequest notification)
        {
            if (notificationId == 0)
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }
            if (notification == null)
            { ModelState.AddModelError(ErrorResponses.MissingBody, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var requestedNotification = new Notification()
            {
                Id = notificationId,
                Status = notification.Status,
                Type = notification.Type,
                Priority = notification.Priority,
                StartDate = notification.StartDate,
                EndDate = notification.EndDate,
                Subject = notification.Subject,
                Message = notification.Message,
                AllowDismissal = notification.AllowDismissal
            };
            var currentUser = await CurrentUserRetriever.GetCurrentUserAsync();
            var updateNotificationCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.UpdateNotification(Logger, currentUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                requestedNotification, notification.SendAsEmail, _queueDataSource, WebApiConfiguration.ServiceBus.Queues.Notifications);
            var resultantCommand = await updateNotificationCommand.Execute(SqlServerDataSource);

            if (resultantCommand.Result != null)
            { return Ok(resultantCommand.Result); }
            else
            { return NotFound(); }
        }

        /// <summary>
        /// Gets a specific <see cref="User"/>'s available tenants.
        /// </summary>
        /// <param name="id">The ID of the <see cref="User"/> to retrieve.</param>
        /// <response code="200" nullable="false">The retrieved <see cref="User"/>'s available tenants.</response>
        /// <response code="400" nullable="false">Unable to get the list of tenants for this <see cref="User"/>, see response for details.</response>
        [Route("{id:int}/tenants")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Jibberwock.DataModels.Tenants.Tenant>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetUserTenants([FromRoute] long id)
        {
            if (id == 0)
            {
                ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getUserTenantsCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.GetTenantsByUser(Logger,
                new User() { Id = id }, false);
            var userTenants = await getUserTenantsCommand.Execute(SqlServerDataSource);

            return Ok(userTenants);
        }
    }
}

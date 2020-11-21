using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Core.API.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : JibberwockControllerBase
    {
        public UserController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

        /// <summary>
        /// Gets the current <see cref="User"/>.
        /// </summary>
        /// <response code="200" nullable="false">The current <see cref="User"/> object.</response>
        [Route("me")]
        [HttpGet]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();

            return Ok(currUser);
        }

        /// <summary>
        /// Gets the current <see cref="User"/>'s <see cref="Notification"/>s.
        /// </summary>
        /// <response code="200" nullable="false">An array of <see cref="Notification"/> containing all notifications for the current <see cref="User"/>.</response>
        [Route("me/notifications")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUserNotifications()
        {
            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();

            var listNotificationsCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.ListClientNotifications(Logger, currUser);
            var allNotifications = await listNotificationsCommand.Execute(SqlServerDataSource);

            return Ok(allNotifications);
        }

        /// <summary>
        /// Dismisses a <see cref="Notification"/> for the current <see cref="User"/>.
        /// </summary>
        /// <param name="notificationId">The ID of the <see cref="Notification"/> to dismiss.</param>
        /// <response code="204" nullable="false">The <see cref="Notification"/> has been dismissed.</response>
        [Route("me/notifications/{notificationId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DismissNotification([FromRoute] long notificationId)
        {
            var currUser = await CurrentUserRetriever.GetCurrentUserAsync();

            var dismissNotificationCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.Dismiss(Logger, currUser, HttpContext.TraceIdentifier, WebApiConfiguration.Authorization.DefaultServiceId, null,
                new Notification() { Id = notificationId });

            await dismissNotificationCommand.Execute(SqlServerDataSource);

            return NoContent();
        }
    }
}

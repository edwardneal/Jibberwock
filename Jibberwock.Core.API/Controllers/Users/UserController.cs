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

            var listGlobalNotificationsCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.ListNotifications(Logger);
            var listUserNotificationsCommand = new Jibberwock.Persistence.DataAccess.Commands.Notifications.ListNotifications(Logger, currUser);

            var allNotifications = await Task.WhenAll(
                listGlobalNotificationsCommand.Execute(SqlServerDataSource),
                listUserNotificationsCommand.Execute(SqlServerDataSource));

            return Ok(allNotifications.SelectMany(n => n));
        }
    }
}

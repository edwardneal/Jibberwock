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

namespace Jibberwock.Admin.API.Controllers.Security
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : JibberwockControllerBase
    {
        public UserController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever) : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

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
            {
                ModelState.AddModelError(ErrorResponses.InvalidFilter, string.Empty);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
        [Route("{id}")]
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

        [Route("{id}/notifications")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetNotifications([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/notifications")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> NotifyUser([FromRoute] string id, [FromBody] object notification)
        {
            return Ok();
        }

        [Route("{id}/notifications/{notificationId}")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateNotification([FromRoute] string id, [FromRoute] string notificationId, [FromBody] object notification)
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetGlobalUserNotifications()
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> NotifyAllUsers([FromBody] object notification)
        {
            return Ok();
        }
    }
}

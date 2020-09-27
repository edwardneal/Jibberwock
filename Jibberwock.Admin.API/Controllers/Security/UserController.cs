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

namespace Jibberwock.Admin.API.Controllers.Security
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : JibberwockControllerBase
    {
        public UserController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource, IOptions<WebApiConfiguration> options) : base(loggerFactory, sqlServerDataSource, options) { }

        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetUsersByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("invalid_filter", "Name filter must have a value.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getUsersCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetUsersByName(Logger, name);

            return Ok(await getUsersCommand.Execute(SqlServerDataSource));
        }

        [Route("{id:int}")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetUserById([FromRoute] long id)
        {
            if (id == 0)
            {
                ModelState.AddModelError("invalid_id", "User ID must be specified.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getSingleUserCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetUserById(Logger, id);

            return Ok(await getSingleUserCommand.Execute(SqlServerDataSource));
        }

        [Route("{id}")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> ControlUserAccess([FromRoute] long id, [FromBody] UserAccessChangeSetting accessChangeSetting)
        {
            if (id == 0)
            { ModelState.AddModelError("invalid_id", "User ID must be specified."); }
            if (accessChangeSetting == null)
            { ModelState.AddModelError("missing_body", "Unable to locate a body for this API call"); }
            
            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var requestedState = new User() { Id = id, Enabled = accessChangeSetting.Enabled };
            var controlUserAccessCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.ControlUserAccess(Logger, requestedState);

            var controlSuccessful = await controlUserAccessCommand.Execute(SqlServerDataSource);

            if (controlSuccessful)
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

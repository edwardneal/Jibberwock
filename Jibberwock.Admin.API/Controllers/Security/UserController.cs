using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Core.Http.Authorization;
using Jibberwock.DataModels.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jibberwock.Admin.API.Controllers.Security
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetUsersByName([FromQuery] string name)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> ControlUserAccess([FromRoute] string id, [FromBody] object accessChangeSettings)
        {
            // enables or disables access
            return Ok();
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

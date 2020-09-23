using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetUsersByName([FromQuery] string name)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> ControlUserAccess([FromRoute] string id, [FromBody] object accessChangeSettings)
        {
            // enables or disables access
            return Ok();
        }

        [Route("{id}/notifications")]
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/notifications")]
        [HttpPost]
        public async Task<IActionResult> NotifyUser([FromRoute] string id, [FromBody] object notification)
        {
            return Ok();
        }

        [Route("{id}/notifications/{notificationId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateNotification([FromRoute] string id, [FromRoute] string notificationId, [FromBody] object notification)
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpGet]
        public async Task<IActionResult> GetGlobalUserNotifications()
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpPost]
        public async Task<IActionResult> NotifyAllUsers([FromBody] object notification)
        {
            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jibberwock.Admin.API.Controllers.Tenants
{
    [ApiController]
    [Route("[controller]")]
    public class OrganisationController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetOrganisationsByName([FromQuery] string name)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetOrganisation([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/roles")]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/roles/{roleId}")]
        [HttpGet]
        public async Task<IActionResult> GetRole([FromRoute] string id, [FromRoute] string roleId)
        {
            return Ok();
        }

        [Route("{id}/roles/{roleId}/members")]
        [HttpPost]
        public async Task<IActionResult> AddRoleMember([FromRoute] string id, [FromRoute] string roleId)
        {
            return Ok();
        }

        [Route("{id}/roles/{roleId}/permissions")]
        [HttpPut]
        public async Task<IActionResult> UpdateRolePermissions([FromRoute] string id, [FromRoute] string roleId)
        {
            return Ok();
        }

        [Route("{id}/subscriptions")]
        [HttpGet]
        public async Task<IActionResult> GetSubscriptions([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/subscriptions/{subscriptionId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSubscription([FromRoute] string id, [FromRoute] string subscriptionId, [FromBody] object subscriptionUpdates)
        {
            return Ok();
        }

        [Route("{id}/subscriptions/{subscriptionId}/statistics")]
        [HttpGet]
        public async Task<IActionResult> GetSubscriptionStatistics([FromRoute] string id, [FromRoute] string subscriptionId)
        {
            return Ok();
        }

        [Route("{id}/audittrail")]
        [HttpGet]
        public async Task<IActionResult> GetAuditTrail([FromRoute] string id, [FromQuery] string start, [FromQuery] string end, [FromQuery] string userId, [FromQuery] string performedBy, [FromQuery] string type)
        {
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
        public async Task<IActionResult> NotifyOrganisation([FromRoute] string id, [FromBody] object notification)
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
        public async Task<IActionResult> GetGlobalOrganisationNotifications()
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpPost]
        public async Task<IActionResult> NotifyAllOrganisations([FromBody] object notification)
        {
            return Ok();
        }
    }
}

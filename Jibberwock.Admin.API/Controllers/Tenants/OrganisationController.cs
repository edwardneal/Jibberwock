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

namespace Jibberwock.Admin.API.Controllers.Tenants
{
    [ApiController]
    [Route("[controller]")]
    public class OrganisationController : JibberwockControllerBase
    {
        public OrganisationController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource) : base(loggerFactory, sqlServerDataSource) { }

        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetOrganisationsByName([FromQuery] string name)
        {
            return Ok();
        }

        [Route("{id}")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetOrganisation([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/roles")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> CreateRole([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/roles/{roleId}")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetRole([FromRoute] string id, [FromRoute] string roleId)
        {
            return Ok();
        }

        [Route("{id}/roles/{roleId}/members")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> AddRoleMember([FromRoute] string id, [FromRoute] string roleId)
        {
            return Ok();
        }

        [Route("{id}/roles/{roleId}/permissions")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateRolePermissions([FromRoute] string id, [FromRoute] string roleId)
        {
            return Ok();
        }

        [Route("{id}/subscriptions")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetSubscriptions([FromRoute] string id)
        {
            return Ok();
        }

        [Route("{id}/subscriptions/{subscriptionId}")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ChangeSubscriptionBilling)]
        public async Task<IActionResult> UpdateSubscription([FromRoute] string id, [FromRoute] string subscriptionId, [FromBody] object subscriptionUpdates)
        {
            return Ok();
        }

        [Route("{id}/subscriptions/{subscriptionId}/statistics")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetSubscriptionStatistics([FromRoute] string id, [FromRoute] string subscriptionId)
        {
            return Ok();
        }

        [Route("{id}/audittrail")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetAuditTrail([FromRoute] string id, [FromQuery] string start, [FromQuery] string end, [FromQuery] string userId, [FromQuery] string performedBy, [FromQuery] string type)
        {
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
        public async Task<IActionResult> NotifyOrganisation([FromRoute] string id, [FromBody] object notification)
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
        public async Task<IActionResult> GetGlobalOrganisationNotifications()
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> NotifyAllOrganisations([FromBody] object notification)
        {
            return Ok();
        }
    }
}

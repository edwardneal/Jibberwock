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
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.DataModels.Tenants;
using Jibberwock.Shared.Http;
using Jibberwock.DataModels.Security.Audit;

namespace Jibberwock.Admin.API.Controllers.Tenants
{
    [ApiController]
    [Route("[controller]")]
    public class TenantController : JibberwockControllerBase
    {
        public TenantController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever) : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

        /// <summary>
        /// Gets all tenants with a name matching a pattern.
        /// </summary>
        /// <param name="name">The pattern to match names against.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="Tenant"/> objects.</response>
        /// <response code="400" nullable="false">Unable to perform the search, see response for details.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Tenant>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetTenantsByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            { ModelState.AddModelError(ErrorResponses.InvalidFilter, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var getTenantsCommand = new Jibberwock.Persistence.DataAccess.Commands.Tenants.GetTenantsByName(Logger, name);

            return Ok(await getTenantsCommand.Execute(SqlServerDataSource));
        }

        [Route("{id:int}")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetTenant([FromRoute] long id)
        {
            return Ok();
        }

        [Route("{id:int}/roles")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> CreateRole([FromRoute] long id)
        {
            return Ok();
        }

        [Route("{id:int}/roles/{roleId:int}")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetRole([FromRoute] long id, [FromRoute] long roleId)
        {
            return Ok();
        }

        [Route("{id:int}/roles/{roleId:int}/members")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> AddRoleMember([FromRoute] long id, [FromRoute] long roleId)
        {
            return Ok();
        }

        [Route("{id:int}/roles/{roleId:int}/permissions")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateRolePermissions([FromRoute] long id, [FromRoute] long roleId)
        {
            return Ok();
        }

        [Route("{id:int}/subscriptions")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetSubscriptions([FromRoute] long id)
        {
            return Ok();
        }

        [Route("{id:int}/subscriptions/{subscriptionId:int}")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ChangeSubscriptionBilling)]
        public async Task<IActionResult> UpdateSubscription([FromRoute] long id, [FromRoute] long subscriptionId, [FromBody] object subscriptionUpdates)
        {
            return Ok();
        }

        [Route("{id:int}/subscriptions/{subscriptionId:int}/statistics")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetSubscriptionStatistics([FromRoute] long id, [FromRoute] long subscriptionId)
        {
            return Ok();
        }

        /// <summary>
        /// Gets an audit trail, filtering it by timestamp, user ID, tenant ID, operator and type.
        /// </summary>
        /// <param name="id">Filter to audit trail entries relating to this tenant.</param>
        /// <param name="start">The start of the time window to retrieve.</param>
        /// <param name="end">The end of the time window to retrieve.</param>
        /// <param name="userId">Filter to audit trail entries relating to this user.</param>
        /// <param name="performedBy">Filter to audit trail entries performed by this user.</param>
        /// <param name="type">Filter to this type of audit trail entry.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="AuditTrailEntry"/> objects.</response>
        [Route("{id:int}/audittrail")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuditTrailEntry>), StatusCodes.Status200OK)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetAuditTrail([FromRoute] long id, [FromQuery] DateTimeOffset? start, [FromQuery] DateTimeOffset? end,
            [FromQuery] long? userId,
            [FromQuery] long? performedBy, [FromQuery] AuditTrailEntryType? type)
        {
            var getAuditTrailCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetAuditTrail(Logger,
                start, end, userId.HasValue ? new DataModels.Users.User() { Id = userId.Value } : null,
                new DataModels.Tenants.Tenant() { Id = id },
                performedBy.HasValue ? new DataModels.Users.User() { Id = performedBy.Value } : null,
                type);
            var auditTrailEntries = await getAuditTrailCommand.Execute(SqlServerDataSource);

            return Ok(auditTrailEntries);
        }

        [Route("{id:int}/notifications")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetNotifications([FromRoute] long id)
        {
            return Ok();
        }

        [Route("{id:int}/notifications")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> NotifyTenant([FromRoute] long id, [FromBody] object notification)
        {
            return Ok();
        }

        [Route("{id:int}/notifications/{notificationId:int}")]
        [HttpPut]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> UpdateNotification([FromRoute] long id, [FromRoute] long notificationId, [FromBody] object notification)
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Read)]
        public async Task<IActionResult> GetGlobalTenantNotifications()
        {
            return Ok();
        }

        [Route("all/notifications")]
        [HttpPost]
        [ResourcePermissions(SecurableResourceType.Service, Permission.Change)]
        public async Task<IActionResult> NotifyAllTenants([FromBody] object notification)
        {
            return Ok();
        }
    }
}

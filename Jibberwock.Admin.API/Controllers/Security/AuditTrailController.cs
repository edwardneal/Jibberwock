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
using Jibberwock.DataModels.Security.Audit;

namespace Jibberwock.Admin.API.Controllers.Security
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditTrailController : JibberwockControllerBase
    {
        public AuditTrailController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever) : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        { }

        /// <summary>
        /// Gets an audit trail, filtering it by timestamp, user ID, tenant ID, operator and type.
        /// </summary>
        /// <param name="start">The start of the time window to retrieve.</param>
        /// <param name="end">The end of the time window to retrieve.</param>
        /// <param name="userId">Filter to audit trail entries relating to this user.</param>
        /// <param name="tenantId">Filter to audit trail entries relating to this tenant.</param>
        /// <param name="performedBy">Filter to audit trail entries performed by this user.</param>
        /// <param name="type">Filter to this type of audit trail entry.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="AuditTrailEntry"/> objects.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuditTrailEntry>), StatusCodes.Status200OK)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetAuditTrail([FromQuery] DateTimeOffset? start, [FromQuery] DateTimeOffset? end,
            [FromQuery] long? userId, [FromQuery] long? tenantId,
            [FromQuery] long? performedBy, [FromQuery] AuditTrailEntryType? type)
        {
            var getAuditTrailCommand = new Jibberwock.Persistence.DataAccess.Commands.Security.GetAuditTrail(Logger,
                start, end, userId.HasValue ? new DataModels.Users.User() { Id = userId.Value } : null,
                tenantId.HasValue ? new DataModels.Tenants.Tenant() { Id = tenantId.Value } : null,
                performedBy.HasValue ? new DataModels.Users.User() { Id = performedBy.Value } : null,
                type);
            var auditTrailEntries = await getAuditTrailCommand.Execute(SqlServerDataSource);

            return Ok(auditTrailEntries);
        }
    }
}

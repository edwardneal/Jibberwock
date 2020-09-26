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

namespace Jibberwock.Admin.API.Controllers.Security
{
    [ApiController]
    [Route("[controller]")]
    public class AuditTrailController : JibberwockControllerBase
    {
        public AuditTrailController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource, IOptions<WebApiConfiguration> options) : base(loggerFactory, sqlServerDataSource, options) { }

        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetAuditTrail([FromQuery] string start, [FromQuery] string end, [FromQuery] string userId, [FromQuery] string organisationId, [FromQuery] string performedBy, [FromQuery] string type)
        {
            return Ok();
        }
    }
}

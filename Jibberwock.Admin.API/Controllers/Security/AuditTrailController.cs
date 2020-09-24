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
    public class AuditTrailController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetAuditTrail([FromQuery] string start, [FromQuery] string end, [FromQuery] string userId, [FromQuery] string organisationId, [FromQuery] string performedBy, [FromQuery] string type)
        {
            return Ok();
        }
    }
}

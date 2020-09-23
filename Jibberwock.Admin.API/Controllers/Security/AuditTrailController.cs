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
    public class AuditTrailController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAuditTrail([FromQuery] string start, [FromQuery] string end, [FromQuery] string userId, [FromQuery] string organisationId, [FromQuery] string performedBy, [FromQuery] string type)
        {
            return Ok();
        }
    }
}

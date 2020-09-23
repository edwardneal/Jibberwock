using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jibberwock.Admin.API.Controllers.Analytics
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        [Route("externalcomponents")]
        [HttpGet]
        public async Task<IActionResult> GetExternalComponentStatuses()
        {
            return Ok();
        }

        [Route("exceptions")]
        [HttpGet]
        public async Task<IActionResult> GetExceptions()
        {
            return Ok();
        }

        [Route("analytics")]
        [HttpGet]
        public async Task<IActionResult> GetAnalytics()
        {
            return Ok();
        }

        [Route("report")]
        [HttpGet]
        public async Task<IActionResult> RedirectToReport()
        {
            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Core.Http.Authorization;
using Jibberwock.DataModels.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jibberwock.Admin.API.Controllers.Analytics
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        [Route("externalcomponents")]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        [HttpGet]
        public async Task<IActionResult> GetExternalComponentStatuses()
        {
            return Ok();
        }

        [Route("exceptions")]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        [HttpGet]
        public async Task<IActionResult> GetExceptions()
        {
            return Ok();
        }

        [Route("analytics")]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        [HttpGet]
        public async Task<IActionResult> GetAnalytics()
        {
            return Ok();
        }

        [Route("report")]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        [HttpGet]
        public async Task<IActionResult> RedirectToReport()
        {
            return Ok();
        }
    }
}

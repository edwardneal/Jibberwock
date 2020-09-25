using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.Shared.Http.Authorization;
using Jibberwock.DataModels.Security;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jibberwock.Admin.API.Controllers.Analytics
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StatusController : JibberwockControllerBase
    {
        public StatusController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource) : base(loggerFactory, sqlServerDataSource) { }

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

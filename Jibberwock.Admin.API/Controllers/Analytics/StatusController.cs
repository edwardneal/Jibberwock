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
using Jibberwock.Shared.Configuration;
using Microsoft.Extensions.Options;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.DataModels.ExternalComponents;
using Jibberwock.Shared.Http;
using Jibberwock.Admin.API.ActionModels.Status;
using Microsoft.Azure.ApplicationInsights;
using System.Security.Cryptography.X509Certificates;

namespace Jibberwock.Admin.API.Controllers.Analytics
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StatusController : JibberwockControllerBase
    {
        private readonly AppInsightsConfiguration _appInsightsConfiguration;

        public StatusController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        {
            _appInsightsConfiguration = options?.Value.AppInsightsConfiguration;
        }

        /// <summary>
        /// Gets all external components and their status.
        /// </summary>
        /// <response code="200" nullable="false">The retrieved set of <see cref="ExternalComponent"/> objects.</response>
        [Route("externalcomponents")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExternalComponent>), StatusCodes.Status200OK)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetExternalComponentStatuses()
        {
            var listAllCommand = new Jibberwock.Persistence.DataAccess.Commands.ExternalComponents.ListAll(Logger);
            var componentStatuses = await listAllCommand.Execute(SqlServerDataSource);

            return Ok(componentStatuses);
        }

        /// <summary>
        /// Gets all exceptions in the last period of time.
        /// </summary>
        /// <response code="200" nullable="false">A <see cref="ServiceExceptionReport"/> containing a list of recent exceptions.</response>
        [Route("exceptions")]
        [HttpGet]
        [ProducesResponseType(typeof(ServiceExceptionReport), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetExceptions()
        {
            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration.AppId))
            { ModelState.AddModelError(ErrorResponses.MisconfiguredApplicationInsightsId, string.Empty); }
            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration.TenantId))
            { ModelState.AddModelError(ErrorResponses.MisconfiguredApplicationInsightsTenant, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            using (var appInsightsClient = await Jibberwock.Shared.Telemetry.ApplicationInsightsDataClientFactory.CreateDataClientAsync(_appInsightsConfiguration.AppId, _appInsightsConfiguration.TenantId))
            {
                var aiExceptionList = await appInsightsClient.GetExceptionEventsAsync(_appInsightsConfiguration.ExceptionTimeRange, cancellationToken: HttpContext.RequestAborted);
                var serviceExceptions = (from aiEx in aiExceptionList.Value
                                         select new ServiceException()
                                         {
                                             Id = aiEx.Id,
                                             Operation = aiEx?.Operation?.Name,
                                             Timestamp = new DateTimeOffset(aiEx.Timestamp.Value, TimeSpan.Zero),
                                             Type = aiEx?.Exception?.Type,
                                             Message = string.IsNullOrWhiteSpace(aiEx?.Exception?.InnermostMessage)
                                                ? aiEx?.Exception?.OuterMessage
                                                : aiEx?.Exception?.InnermostMessage
                                         }).OrderBy(e => e.Timestamp).ToArray();
                var serviceReport = new ServiceExceptionReport()
                {
                    StartDate = DateTimeOffset.UtcNow - _appInsightsConfiguration.ExceptionTimeRange,
                    EndDate = DateTimeOffset.UtcNow,
                    Exceptions = serviceExceptions
                };

                return Ok(serviceReport);
            }
        }

        /// <summary>
        /// Redirects to the status reporting page.
        /// </summary>
        /// <response code="302" nullable="false">A redirect to the status reporting page.</response>
        [Route("report")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public IActionResult RedirectToReport()
        {
            return Redirect(WebApiConfiguration.PermittedRedirects["status_report"]);
        }
    }
}

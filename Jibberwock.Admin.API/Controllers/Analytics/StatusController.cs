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
using Jibberwock.DataModels.Core;

namespace Jibberwock.Admin.API.Controllers.Analytics
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration?.AppId))
            { ModelState.AddModelError(ErrorResponses.MisconfiguredApplicationInsightsId, string.Empty); }
            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration?.TenantId))
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
                                                : aiEx?.Exception?.InnermostMessage,
                                             RoleName = aiEx.Cloud.RoleName,
                                             UserId = aiEx.User.Id,
                                             SessionId = aiEx.Session.Id,
                                             Source = aiEx.Ai.SdkVersion.Split(':').FirstOrDefault()?.Trim()
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
        /// Gets all failed HTTP requests in the last period of time.
        /// </summary>
        /// <response code="200" nullable="false">A <see cref="FailedRequestReport"/> containing a list of recent failed HTTP requests.</response>
        [Route("requests")]
        [HttpGet]
        [ProducesResponseType(typeof(FailedRequestReport), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetFailedRequests()
        {
            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration?.AppId))
            { ModelState.AddModelError(ErrorResponses.MisconfiguredApplicationInsightsId, string.Empty); }
            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration?.TenantId))
            { ModelState.AddModelError(ErrorResponses.MisconfiguredApplicationInsightsTenant, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            using (var appInsightsClient = await Jibberwock.Shared.Telemetry.ApplicationInsightsDataClientFactory.CreateDataClientAsync(_appInsightsConfiguration.AppId, _appInsightsConfiguration.TenantId))
            {
                var aiRequestList = await appInsightsClient.GetRequestEventsAsync(_appInsightsConfiguration.ExceptionTimeRange, filter: "startswith(request/resultCode, '5')", cancellationToken: HttpContext.RequestAborted);
                var failedRequests = (from aiR in aiRequestList.Value
                                      select new FailedRequest()
                                      {
                                          Id = aiR.Id,
                                          Timestamp = new DateTimeOffset(aiR.Timestamp.Value, TimeSpan.Zero),
                                          RoleName = aiR.Cloud.RoleName,
                                          DurationMS = aiR.Request?.Duration,
                                          Name = aiR.Request.Name,
                                          DurationBucket = aiR.Request?.PerformanceBucket,
                                          ResultCode = aiR.Request.ResultCode
                                      }).OrderBy(r => r.Timestamp).ToArray();
                var requestReport = new FailedRequestReport()
                {
                    StartDate = DateTimeOffset.UtcNow - _appInsightsConfiguration.ExceptionTimeRange,
                    EndDate = DateTimeOffset.UtcNow,
                    FailedRequests = failedRequests
                };

                return Ok(requestReport);
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

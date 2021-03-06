﻿using Jibberwock.Admin.API.ActionModels.Notifications;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Security;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Jibberwock.Shared.Http;
using Jibberwock.Shared.Http.Authentication;
using Jibberwock.Shared.Http.Authorization;
using Jibberwock.Shared.Http.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.Controllers.Notifications
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmailController : JibberwockControllerBase
    {
        private readonly Jibberwock.Persistence.DataAccess.Utility.Interfaces.IHashCalculator _hashCalculator;
        private readonly AppInsightsConfiguration _appInsightsConfiguration;

        public EmailController(ILoggerFactory loggerFactory, SqlServerDataSource sqlServerDataSource,
            IOptions<WebApiConfiguration> options, ICurrentUserRetriever currentUserRetriever,
            Jibberwock.Persistence.DataAccess.Utility.Interfaces.IHashCalculator hashCalculator)
            : base(loggerFactory, sqlServerDataSource, options, currentUserRetriever)
        {
            _hashCalculator = hashCalculator;
            _appInsightsConfiguration = options?.Value.AppInsightsConfiguration;
        }

        /// <summary>
        /// Gets all historical email batches.
        /// </summary>
        /// <response code="200" nullable="false">The retrieved set of <see cref="EmailBatch"/> objects.</response>
        [Route("batches")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmailBatch>), StatusCodes.Status200OK)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> ListBatches()
        {
            var listBatchesCommand = new Jibberwock.Persistence.DataAccess.Commands.Emails.ListBatches(Logger);
            var batches = await listBatchesCommand.Execute(SqlServerDataSource);

            return Ok(batches);
        }

        /// <summary>
        /// Gets all emails sent, filtering them by timestamp, email batch, batch type, service bus message ID and email address.
        /// </summary>
        /// <param name="start">The start of the time window to retrieve.</param>
        /// <param name="end">The end of the time window to retrieve.</param>
        /// <param name="batchId">The internal ID of the email batch to retrieve.</param>
        /// <param name="batchTypeId">The type of every email batch for the message to retrieve.</param>
        /// <param name="serviceBusMessageId">The Service Bus message ID of the email batch containing the emails to retrieve.</param>
        /// <param name="emailAddress">The To: address of the messages to retrieve.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="EmailBatch"/> objects.</response>
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmailBatch>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetEmailHistory([FromQuery] DateTimeOffset? start, [FromQuery] DateTimeOffset? end,
            [FromQuery] long? batchId, [FromQuery] int? batchTypeId,
            [FromQuery] string serviceBusMessageId, [FromQuery] string emailAddress)
        {
            if (!string.IsNullOrWhiteSpace(emailAddress) && !start.HasValue)
            { ModelState.AddModelError(ErrorResponses.MissingStartTime, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            var getEmailHistoryCommand = new Jibberwock.Persistence.DataAccess.Commands.Emails.GetEmailHistory(
                Logger, _hashCalculator, start, end,
                batchId.HasValue ? new EmailBatch() { Id = batchId.Value } : null,
                batchTypeId.HasValue && !batchId.HasValue ? new EmailBatchType() { Id = batchTypeId.Value } : null,
                serviceBusMessageId, emailAddress);

            var emailHistory = await getEmailHistoryCommand.Execute(SqlServerDataSource);

            return Ok(emailHistory);
        }

        /// <summary>
        /// Get all events for this particular email.
        /// </summary>
        /// <param name="externalEmailId">The ID of this email in the external email system.</param>
        /// <response code="200" nullable="false">The retrieved set of <see cref="EmailEvent"/> objects.</response>
        [Route("events")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmailEvent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResourcePermissions(SecurableResourceType.Service, Permission.ReadLogs)]
        public async Task<IActionResult> GetEvents([FromQuery] string externalEmailId)
        {
            if (string.IsNullOrWhiteSpace(externalEmailId))
            { ModelState.AddModelError(ErrorResponses.InvalidId, string.Empty); }

            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration?.AppId))
            { ModelState.AddModelError(ErrorResponses.MisconfiguredApplicationInsightsId, string.Empty); }
            if (string.IsNullOrWhiteSpace(_appInsightsConfiguration?.TenantId))
            { ModelState.AddModelError(ErrorResponses.MisconfiguredApplicationInsightsTenant, string.Empty); }

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); }

            using (var appInsightsClient = await Jibberwock.Shared.Telemetry.ApplicationInsightsDataClientFactory.CreateDataClientAsync(_appInsightsConfiguration.AppId, _appInsightsConfiguration.TenantId))
            {
                // All of these events are stored in Application Insights - so query that and return them
                // Unfortunately the API doesn't allow us to access CustomDimensions very well, so work with the response directly if required
                var oDataFilter = $"customDimensions/{WebApiConfiguration.SendGrid.EmailIdParameterName} eq '{externalEmailId.Replace("'", "''")}'";
                var aiEventList = await appInsightsClient.GetCustomEventsWithHttpMessagesAsync(filter: oDataFilter, cancellationToken: HttpContext.RequestAborted);
                var rawResponse = await aiEventList.Response.Content.ReadAsStringAsync();
                var parsedResponse = JsonDocument.Parse(rawResponse);

                var emailEvents = (from evt in aiEventList.Body.Value
                                   let propsDict = getEventDimensions(parsedResponse, evt.Id)
                                   let timestamp = evt.Timestamp.HasValue ? new DateTimeOffset(evt.Timestamp.Value) : DateTimeOffset.MinValue
                                   select new EmailEvent()
                                   {
                                       Type = propsDict["sendgrid_event_type"],
                                       SmtpMessageId = propsDict.ContainsKey("smtp_message_id") ? propsDict["smtp_message_id"] : null,
                                       Timestamp = timestamp,
                                       SmtpBounceReason = propsDict.ContainsKey("smtp_bounce_reason") ? propsDict["smtp_bounce_reason"] : null,
                                       SmtpBounceType = propsDict.ContainsKey("smtp_bounce_type") ? propsDict["smtp_bounce_type"] : null,
                                       SmtpDroppedReason = propsDict.ContainsKey("smtp_dropped_reason") ? propsDict["smtp_dropped_reason"] : null,
                                       SmtpDeferredResponse = propsDict.ContainsKey("smtp_deferred_response") ? propsDict["smtp_deferred_response"] : null
                                   }).ToArray();

                return Ok(emailEvents);
            }
        }

        private static Dictionary<string, string> getEventDimensions(JsonDocument response, string eventId)
        {
            var valueArray = response.RootElement.GetProperty("value");

            foreach(var aiEvent in valueArray.EnumerateArray())
            {
                var aiId = aiEvent.GetProperty("id").GetString();

                if (eventId.Equals(aiId, StringComparison.OrdinalIgnoreCase))
                {
                    return aiEvent.GetProperty("customDimensions")
                        .EnumerateObject()
                        .ToDictionary(p => p.Name.ToLower(), p => p.Value.GetString());
                }
            }

            return null;
        }
    }
}

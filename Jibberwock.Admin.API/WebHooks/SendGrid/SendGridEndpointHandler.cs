using Jibberwock.Shared.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Metrics.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sendgrid.Webhooks.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.WebHooks.SendGrid
{
    public class SendGridEndpointHandler
    {
        public const string EndpointPattern = "/webhook/sendgrid";

        public static async Task HandleSendGridWebHook(HttpContext httpContext)
        {
            var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var webApiConfiguration = httpContext.RequestServices.GetRequiredService<IOptions<WebApiConfiguration>>()?.Value;
            var appInsightsTelemetry = httpContext.RequestServices.GetRequiredService<TelemetryClient>();
            var logger = loggerFactory.CreateLogger<SendGridEndpointHandler>();
            var sendgridValidator = new global::SendGrid.Helpers.EventWebhook.RequestValidator();
            var sendgridParser = new Sendgrid.Webhooks.Service.WebhookParser();

            logger.LogDebug("Entered SendGrid webhook");

            using var sr = new StreamReader(httpContext.Request.Body);
            var requestBody = await sr.ReadToEndAsync();

            logger.LogDebug($"Successfully read webhook body ({(requestBody ?? string.Empty).Length} bytes)");

            // If we've got a verification public key (and we should!), we want to verify the web hook is from SendGrid
            if (!string.IsNullOrWhiteSpace(webApiConfiguration?.SendGrid?.VerificationPublicKey))
            {
                var parsedVerificationKey = sendgridValidator.ConvertPublicKeyToECDSA(webApiConfiguration.SendGrid.VerificationPublicKey);

                if (httpContext.Request.Headers.TryGetValue(global::SendGrid.Helpers.EventWebhook.RequestValidator.SIGNATURE_HEADER, out var whSignatureHeader)
                    && whSignatureHeader.Count > 0
                    && httpContext.Request.Headers.TryGetValue(global::SendGrid.Helpers.EventWebhook.RequestValidator.TIMESTAMP_HEADER, out var whTimestampHeader)
                    && whTimestampHeader.Count > 0)
                {
                    var whSignature = whSignatureHeader[0];
                    var whTimestamp = whTimestampHeader[0];
                    var whVerification = sendgridValidator.VerifySignature(parsedVerificationKey, requestBody, whSignature, whTimestamp);

                    // If verification is enabled and fails, we definitely don't want to parse the web hook - it's fraudulent!
                    if (whVerification)
                    { logger.LogInformation("Webhook was verified successfully."); }
                    else
                    {
                        logger.LogWarning($"Webhook failed validation. Expected signature was \"{whSignature}\". Timestamp header was \"{whTimestamp}\"");
                        throw new InvalidOperationException("Webhook failed validation.");
                    }
                }
                else
                { throw new InvalidOperationException("Cannot process webhook: signature and timestamp headers are not present."); }
            }
            else
            { logger.LogWarning("Webhook body will not be verified - the verification public key is missing."); }

            var hookEvents = sendgridParser.ParseEvents(requestBody);

            logger.LogInformation($"Parsed SendGrid webhook body: {hookEvents.Count} event(s) found");

            foreach(var webhookEvent in hookEvents)
            {
                if (webhookEvent == null)
                {
                    logger.LogWarning("SendGrid webhook body contained a null event.");
                    continue;
                }
                try
                {
                    logger.LogInformation($"Processing SendGrid '{webhookEvent.EventType}' event (ID: \"{webhookEvent.SgEventId}\")");

                    var eventTelemetry = new Microsoft.ApplicationInsights.DataContracts.EventTelemetry($"SendGrid Event: {webhookEvent.EventType}")
                    // Tie the timstamp to the webhook event timestamp. This is in UTC
                    { Timestamp = new DateTimeOffset(webhookEvent.Timestamp, TimeSpan.Zero) };

                    // Always include the SendGrid identifiers (for the sake of traceability and querying)
                    eventTelemetry.Properties.Add("sendgrid_event_id", webhookEvent.SgEventId);
                    eventTelemetry.Properties.Add("sendgrid_message_id", webhookEvent.SgMessageId);
                    eventTelemetry.Properties.Add("sendgrid_event_type", webhookEvent.EventType.ToString());

                    if (webhookEvent as DeliveryEventBase != null)
                    { eventTelemetry.Properties.Add("smtp_message_id", (webhookEvent as DeliveryEventBase).SmtpId); }

                    if (webhookEvent as BounceEvent != null)
                    {
                        eventTelemetry.Properties.Add("smtp_bounce_reason", (webhookEvent as BounceEvent).Reason);
                        eventTelemetry.Properties.Add("smtp_bounce_type", (webhookEvent as BounceEvent).BounceType);
                    }

                    if (webhookEvent as DroppedEvent != null)
                    { eventTelemetry.Properties.Add("smtp_dropped_reason", (webhookEvent as DroppedEvent).Reason); }

                    if (webhookEvent as DeferredEvent != null)
                    { eventTelemetry.Properties.Add("smtp_deferred_response", (webhookEvent as DeferredEvent).Response); }

                    // We'll include the "jibberwock_notification_id" as a unique parameter, to trace this from the background service
                    if (webhookEvent.UniqueParameters.ContainsKey(webApiConfiguration.SendGrid.NotificationIdParameterName))
                    { eventTelemetry.Properties.Add(webApiConfiguration.SendGrid.NotificationIdParameterName, webhookEvent.UniqueParameters[webApiConfiguration.SendGrid.NotificationIdParameterName]); }

                    appInsightsTelemetry.TrackEvent(eventTelemetry);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Unable to process SendGrid '{webhookEvent.EventType}' event (ID: \"{webhookEvent.SgEventId}\", for SendGrid message \"{webhookEvent.SgMessageId}\")");
                }
            }

            logger.LogDebug("Left SendGrid webhook");
        }
    }
}

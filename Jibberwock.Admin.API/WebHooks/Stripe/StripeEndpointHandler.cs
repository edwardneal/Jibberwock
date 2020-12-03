using Jibberwock.Shared.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.WebHooks.Stripe
{
    public class StripeEndpointHandler
    {
        public const string EndpointPattern = "/webhook/stripe";

        private const string SignatureHeader = "Stripe-Signature";

        public static async Task HandleStripeWebHook(HttpContext httpContext)
        {
            var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var webApiConfiguration = httpContext.RequestServices.GetRequiredService<IOptions<WebApiConfiguration>>()?.Value;
            var appInsightsTelemetry = httpContext.RequestServices.GetRequiredService<TelemetryClient>();
            var stripeEventHub = httpContext.RequestServices.GetRequiredService<StripeEventHub>();
            var logger = loggerFactory.CreateLogger<StripeEndpointHandler>();
            Event resultantEvent;

            logger.LogDebug("Entered Stripe webhook");

            using var sr = new StreamReader(httpContext.Request.Body);
            var requestBody = await sr.ReadToEndAsync();

            logger.LogDebug($"Successfully read webhook body ({(requestBody ?? string.Empty).Length} bytes)");

            // Just like the SendGrid hook, check the signature on the webhook if we can
            if (!string.IsNullOrWhiteSpace(webApiConfiguration.Stripe.WebHookSecret))
            {
                if (httpContext.Request.Headers.TryGetValue(SignatureHeader, out var whSignatureHeader)
                    && whSignatureHeader.Count > 0)
                {
                    var whSignature = whSignatureHeader[0];

                    // Try to construct the event from the known signature. Pass any exceptions through to make the client fail.
                    try
                    {
                        resultantEvent = EventUtility.ConstructEvent(requestBody, whSignature, webApiConfiguration.Stripe.WebHookSecret);
                        logger.LogInformation("Webhook was verified successfully.");
                    }
                    catch(StripeException sE)
                    {
                        logger.LogWarning(sE, "Webhook failed validation. Exception details are attached.");
                        throw;
                    }
                }
                else
                { throw new InvalidOperationException("Cannot process webhook: signature header is not present."); }
            }
            else
            {
                logger.LogWarning("Webhook body will not be verified - the webhook secret is missing.");
                resultantEvent = EventUtility.ParseEvent(requestBody);
            }

            if (resultantEvent == null)
            {
                logger.LogWarning("Parsed Stripe event was null.");
                return;
            }

            logger.LogInformation($"Parsed Stripe webhook body: {resultantEvent.Type} event found. Processing.");

            try
            {
                // Send a generic record of events to Application Insights. Include the events' IDs and types
                var eventTelemetry = new Microsoft.ApplicationInsights.DataContracts.EventTelemetry($"Stripe Event: {resultantEvent.Type}")
                // The event timestamp needs to align between the webhook event and the Application Insights event (in UTC)
                { Timestamp = new DateTimeOffset(resultantEvent.Created, TimeSpan.Zero) };

                eventTelemetry.Properties.Add("stripe_event_id", resultantEvent.Id);
                eventTelemetry.Properties.Add("stripe_event_type", resultantEvent.Type);

                if (resultantEvent?.Data?.Object != null)
                { eventTelemetry.Properties.Add("stripe_object_type", resultantEvent.Data.Object.Object); }

                if ((resultantEvent?.Data?.Object as IHasId) != null)
                { eventTelemetry.Properties.Add("stripe_object_id", (resultantEvent.Data.Object as IHasId).Id); }

                // First, write this event out to Application Insights
                appInsightsTelemetry.TrackEvent(eventTelemetry);

                logger.LogInformation("Invoking Jibberwock event-specific handlers.");
                // Then, provide the Jibberwock-specific handling
                await stripeEventHub.RaiseEvent(httpContext.RequestServices, resultantEvent);
                logger.LogInformation("Jibberwock event-specific handlers invoked.");
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Unable to process Stripe '{resultantEvent.Type}' event (ID: \"{resultantEvent.Id}\")");
            }

            logger.LogDebug("Left Stripe webhook");
        }
    }
}

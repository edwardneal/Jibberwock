using Microsoft.Azure.ApplicationInsights;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Shared.Telemetry
{
    /// <summary>
    /// Creates an <see cref="ApplicationInsightsDataClient"/>, authenticated using Azure AD (without a client secret.)
    /// </summary>
    public static class ApplicationInsightsDataClientFactory
    {
        private const string ApplicationInsightsResource = "https://api.applicationinsights.io/";

        /// <summary>
        /// Creates an <see cref="ApplicationInsightsDataClient"/>, authenticated using Azure AD.
        /// </summary>
        /// <param name="applicationInsightsId">The AppID of the Application Insights resource.</param>
        /// <param name="tenantId">The tenant containing the Application Insights resource.</param>
        /// <returns>An authenticated <see cref="ApplicationInsightsDataClient"/>.</returns>
        public static async Task<ApplicationInsightsDataClient> CreateDataClientAsync(string applicationInsightsId, string tenantId)
        {
            // Get an access token and wrap it up into some TokenCredentials, then pass it to ApplicationInsightsDataClient.
            var tokenProvider = new AzureServiceTokenProvider();
            var accessToken = await tokenProvider.GetAccessTokenAsync(ApplicationInsightsResource, tenantId);
            var wrappedTokenCredentials = new TokenCredentials(accessToken);

            return new ApplicationInsightsDataClient(wrappedTokenCredentials) { AppId = applicationInsightsId };
        }
    }
}

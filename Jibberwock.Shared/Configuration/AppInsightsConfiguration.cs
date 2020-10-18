using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration
{
    /// <summary>
    /// Contains the high-level configuration for Application Insights.
    /// </summary>
    public class AppInsightsConfiguration
    {
        /// <summary>
        /// The ID of the tenant which the Application Insights instance resides in.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// The ID of the Application Insights instance.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// The time range to find exceptions from.
        /// </summary>
        public TimeSpan ExceptionTimeRange { get; set; }
    }
}

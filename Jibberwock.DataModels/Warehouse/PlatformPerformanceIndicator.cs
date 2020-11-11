using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Warehouse
{
    /// <summary>
    /// This represents an aggregated summary of the current state of the Jibberwock platform.
    /// </summary>
    public class PlatformPerformanceIndicator
    {
        /// <summary>
        /// The total number of users in the platform.
        /// </summary>
        public long UserCount { get; set; }

        /// <summary>
        /// The total number of tenants in the platform.
        /// </summary>
        public long TenantCount { get; set; }

        /// <summary>
        /// If <c>true</c>, all external components are healthy.
        /// </summary>
        public bool ComponentsHealthy { get; set; }

        /// <summary>
        /// The number of email batches which have not yet been processed.
        /// </summary>
        public long PendingEmailBatches { get; set; }

        /// <summary>
        /// Total number of entries in the audit trail, broken down by date.
        /// </summary>
        public Dictionary<DateTime, long> ActivityByDate { get; set; }
    }
}

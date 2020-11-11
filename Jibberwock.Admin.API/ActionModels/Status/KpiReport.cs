using Jibberwock.DataModels.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Status
{
    /// <summary>
    /// Describes a report of key platform status/performance indicators.
    /// </summary>
    public class KpiReport
    {
        /// <summary>
        /// The start date of this report.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// The end date of this report.
        /// </summary>
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// KPIs for the period between <see cref="StartDate"/> and <see cref="EndDate"/>.
        /// </summary>
        public PlatformPerformanceIndicator Kpi { get; set; }
    }
}

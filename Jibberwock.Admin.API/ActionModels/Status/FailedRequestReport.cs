using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Status
{
    /// <summary>
    /// Describes a report of failed HTTP requests between a start and an end date.
    /// </summary>
    public class FailedRequestReport
    {
        /// <summary>
        /// The start date of this report.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// End date of this report.
        /// </summary>
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// All requests recorded between <see cref="StartDate"/> and <see cref="EndDate"/>.
        /// </summary>
        public IEnumerable<FailedRequest> FailedRequests { get; set; }
    }
}

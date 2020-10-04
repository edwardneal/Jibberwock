using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Status
{
    /// <summary>
    /// Describes a report of platform errors between a start and an end date.
    /// </summary>
    public class ServiceExceptionReport
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
        /// All exceptions recorded between <see cref="StartDate"/> and <see cref="EndDate"/>.
        /// </summary>
        public IEnumerable<ServiceException> Exceptions { get; set; }
    }
}

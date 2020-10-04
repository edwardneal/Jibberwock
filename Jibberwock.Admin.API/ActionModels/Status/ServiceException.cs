using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Status
{
    /// <summary>
    /// Describes an error experienced by the platform.
    /// </summary>
    public class ServiceException
    {
        /// <summary>
        /// Unique identifier within Application Insights of this exception.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// UTC time when the exception occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The operation which triggered the exception. If triggered by a HTTP request, this will be in the form "{verb} {path}"
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// The type of this exception.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Message associated with this exception.
        /// </summary>
        public string Message { get; set; }
    }
}

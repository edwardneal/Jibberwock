using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Status
{
    /// <summary>
    /// Describes a failed HTTP request (result code of 5xx) returned by the platform.
    /// </summary>
    public class FailedRequest
    {
        /// <summary>
        /// Unique identifier within Application Insights of this request.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// UTC time when the request occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// If present, the name of the Azure resource which handled the response.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Duration of the request (in milliseconds.)
        /// </summary>
        public double? DurationMS { get; set; }

        /// <summary>
        /// The HTTP request name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The generic bucket which the duration falls into.
        /// </summary>
        public string DurationBucket { get; set; }

        /// <summary>
        /// The HTTP response code which was issued.
        /// </summary>
        public string ResultCode { get; set; }
    }
}

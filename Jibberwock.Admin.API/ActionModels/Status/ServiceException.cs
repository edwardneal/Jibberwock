using Microsoft.AspNetCore.Identity;
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

        /// <summary>
        /// If present, the name of the Azure resource which triggered the error.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// A unique identifier for the person who triggered the error.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// A unique identifier for the session.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// The soruce of this exception.
        /// </summary>
        public string Source { get; set; }
    }
}

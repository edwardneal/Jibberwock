using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.ExternalComponents.Status
{
    /// <summary>
    /// This class stores a component's status, if it can be retrieved programmatically.
    /// </summary>
    public class ExternalComponentStatus
    {
        /// <summary>
        /// The method which will be used to retrieve this component's status.
        /// </summary>
        public StatusProvider StatusProvider { get; set; }

        /// <summary>
        /// If true, the parent <see cref="ExternalComponent"/> is available for use.
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// The "raw" third-party status, if programmatically retrieved.
        /// </summary>
        public string ThirdPartyStatus { get; set; }

        /// <summary>
        /// The date/time where this component's status was retrieved. Null if the third-party status cannot be programmatically retrieved.
        /// </summary>
        public DateTimeOffset? RetrievalDate { get; set; }
    }
}

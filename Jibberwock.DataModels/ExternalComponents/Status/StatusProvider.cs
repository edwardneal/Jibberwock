using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.ExternalComponents.Status
{
    /// <summary>
    /// A third-party service which provides the current status of an external component.
    /// </summary>
    public enum StatusProvider
    {
        /// <summary>
        /// No status provider exists, or the status provider does not expose an API to retrieve the status of this component.
        /// </summary>
        NotApplicable = 0,
        /// <summary>
        /// Atlassian Status. The status of this component can be retrieved programmatically.
        /// </summary>
        AtlassianStatus = 1
    }
}

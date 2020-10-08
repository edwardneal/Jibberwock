using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// The status of a given <see cref="Notification"/>.
    /// </summary>
    public enum NotificationStatus
    {
        /// <summary>
        /// This <see cref="Notification"/> is active and should be processed.
        /// </summary>
        Active = 1,
        /// <summary>
        /// This <see cref="Notification"/> has been cancelled. It should not be processed.
        /// </summary>
        Cancelled = 2
    }
}

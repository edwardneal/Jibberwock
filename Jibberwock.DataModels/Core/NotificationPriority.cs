using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// Represents the priority of a specific <see cref="Notification"/>.
    /// </summary>
    /// <remarks>
    /// If this <see cref="Notification"/> generates an email with a "high" priority
    /// notification, the email will be flagged as high priority.
    /// </remarks>
    public class NotificationPriority
    {
        /// <summary>
        /// The unique internal identifier for this <see cref="NotificationPriority"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of this <see cref="NotificationPriority"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The order in which this <see cref="NotificationPriority"/> should appear in the list.
        /// </summary>
        public int Order { get; set; }
    }
}

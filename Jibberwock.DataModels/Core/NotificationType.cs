using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// The type of a given <see cref="Notification"/>.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// This <see cref="Notification"/> is an important piece of information which
        /// requires action from a person (but might not have been triggered by any person.)
        /// </summary>
        Alert = 1,
        /// <summary>
        /// This <see cref="Notification"/> is informational in nature. It does not require
        /// any action from a person.
        /// </summary>
        Information = 2,
        /// <summary>
        /// This <see cref="Notification"/> is reporting a service-level error to a person and
        /// requires specific, immediate action.
        /// </summary>
        Error = 3,
        /// <summary>
        /// This <see cref="Notification"/> is reporting a service-level warning to a person. It
        /// might require specific action, but this action does not need to be immediate.
        /// </summary>
        Warning = 4
    }
}

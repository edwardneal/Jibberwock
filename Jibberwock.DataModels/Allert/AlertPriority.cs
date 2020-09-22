using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert
{
    /// <summary>
    /// Represents the mapping of <see cref="Alert.Priority"/> to a friendly priority name.
    /// </summary>
    public class AlertPriority
    {
        /// <summary>
        /// The friendly name of an <see cref="Alert"/>'s priority.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If applicable, a meaningful description of what this priority means.
        /// </summary>
        public string Description { get; set; }
    }
}

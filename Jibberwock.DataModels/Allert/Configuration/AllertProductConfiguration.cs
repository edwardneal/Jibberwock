using Jibberwock.DataModels.Products.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert.Configuration
{
    /// <summary>
    /// Allert-specific product configuration.
    /// </summary>
    public class AllertProductConfiguration : ProductConfigurationBase
    {
        /// <summary>
        /// Language string mappings for all <see cref="Alert"/> priorities.
        /// </summary>
        public IDictionary<int, AlertPriority> AlertPriorityNames { get; set; }
    }
}

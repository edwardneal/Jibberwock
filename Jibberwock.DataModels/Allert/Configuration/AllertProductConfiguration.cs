using Jibberwock.DataModels.Products.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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

        public override string ConfigurationString
        {
            get
            {
                return JsonSerializer.Serialize(new { AlertPriorityNames = AlertPriorityNames as Dictionary<int, AlertPriority> });
            }
            set
            {
                var jsonDocument = JsonDocument.Parse(value);
                var alertPriorityNames = jsonDocument.RootElement.GetProperty(nameof(AlertPriorityNames)).GetRawText();

                AlertPriorityNames = JsonSerializer.Deserialize<Dictionary<int, AlertPriority>>(alertPriorityNames);
            }
        }

        public AllertProductConfiguration(ProductConfigurationBase sourceConfiguration)
            :base(sourceConfiguration)
        { }
    }
}

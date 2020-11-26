using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Products.Configuration
{
    /// <summary>
    /// This class is derived from <see cref="ProductConfigurationBase"/>, and enables raw access to the <see cref="ConfigurationString"/> field.
    /// </summary>
    public class RawProductConfiguration : ProductConfigurationBase
    {
        public RawProductConfiguration(ProductConfigurationBase sourceConfiguration)
            : base(sourceConfiguration)
        { }

        public RawProductConfiguration(string configurationString)
            :base()
        {
            ConfigurationString = configurationString;
        }

        public override string ConfigurationString { get; set; }
    }
}

using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies a <see cref="ProductCharacteristic"/>
    /// </summary>
    public class ModifyProductCharacteristic : AuditTrailEntry
    {
        public ModifyProductCharacteristic()
            : base()
        {
            Type = AuditTrailEntryType.ModifyProductCharacteristic;
        }

        /// <summary>
        /// The new state of the <see cref="ProductCharacteristic"/> being modified or created.
        /// </summary>
        public ProductCharacteristic ProductCharacteristic { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="ProductCharacteristic"/> is a new record.
        /// </summary>
        public bool NewProductCharacteristic { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { ProductCharacteristic, NewProductCharacteristic });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewProductCharacteristic = jsonDoc.RootElement.GetProperty(nameof(NewProductCharacteristic)).GetBoolean();
                ProductCharacteristic = JsonSerializer.Deserialize<ProductCharacteristic>(jsonDoc.RootElement.GetProperty(nameof(ProductCharacteristic)).GetRawText());
            }
        }
    }
}

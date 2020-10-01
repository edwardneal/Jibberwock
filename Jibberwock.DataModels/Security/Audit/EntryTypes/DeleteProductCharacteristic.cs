using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which deletes a <see cref="ProductCharacteristic"/>
    /// </summary>
    public class DeleteProductCharacteristic : AuditTrailEntry
    {
        public DeleteProductCharacteristic()
            : base()
        {
            Type = AuditTrailEntryType.DeleteProductCharacteristic;
        }

        /// <summary>
        /// The state of the <see cref="ProductCharacteristic"/> being deleted.
        /// </summary>
        public ProductCharacteristic ProductCharacteristic { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(ProductCharacteristic);
            set
            {
                ProductCharacteristic = JsonSerializer.Deserialize<ProductCharacteristic>(value);
            }
        }
    }
}

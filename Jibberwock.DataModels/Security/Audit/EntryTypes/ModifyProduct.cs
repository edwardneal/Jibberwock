using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which creates or modifies a <see cref="Product"/>
    /// </summary>
    public class ModifyProduct : AuditTrailEntry
    {
        public ModifyProduct()
            :base()
        {
            Type = AuditTrailEntryType.ModifyProduct;
        }

        /// <summary>
        /// The new state of the <see cref="Product"/> being modified or created.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="Product"/> is a new record.
        /// </summary>
        public bool NewProduct { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(new { Product, NewProduct });
            set
            {
                var jsonDoc = JsonDocument.Parse(value);

                NewProduct = jsonDoc.RootElement.GetProperty(nameof(NewProduct)).GetBoolean();
                Product = JsonSerializer.Deserialize<Product>(jsonDoc.RootElement.GetProperty(nameof(Product)).GetRawText());
            }
        }
    }
}

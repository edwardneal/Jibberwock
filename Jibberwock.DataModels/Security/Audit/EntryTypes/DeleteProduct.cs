using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.DataModels.Security.Audit.EntryTypes
{
    /// <summary>
    /// Represents an audit trail entry which deletes a <see cref="Product"/>
    /// </summary>
    public class DeleteProduct : AuditTrailEntry
    {
        public DeleteProduct()
            :base()
        {
            Type = AuditTrailEntryType.DeleteProduct;
        }

        /// <summary>
        /// The state of the <see cref="Product"/> being deleted.
        /// </summary>
        public Product Product { get; set; }

        public override string Metadata
        {
            get => JsonSerializer.Serialize(Product);
            set
            {
                Product = JsonSerializer.Deserialize<Product>(value);
            }
        }
    }
}

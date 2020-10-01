using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

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

        public override string Metadata { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

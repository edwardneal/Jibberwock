using Jibberwock.DataModels.Products;
using System;
using System.Collections.Generic;
using System.Text;

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

        public override string Metadata { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

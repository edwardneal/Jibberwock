﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security.Audit
{
    /// <summary>
    /// Describes the type of entry in the audit trail described by an <see cref="AuditTrailEntry"/>.
    /// </summary>
    public enum AuditTrailEntryType
    {
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.ControlUserAccess"/> record.
        /// </summary>
        ControlUserAccess = 1,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyProduct"/> record.
        /// </summary>
        ModifyProduct = 2,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.DeleteProduct"/> record.
        /// </summary>
        DeleteProduct = 3,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyProductCharacteristic"/> record.
        /// </summary>
        ModifyProductCharacteristic = 4,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.DeleteProductCharacteristic"/> record.
        /// </summary>
        DeleteProductCharacteristic = 5
    }
}

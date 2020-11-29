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
        DeleteProductCharacteristic = 5,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyTier"/> record.
        /// </summary>
        ModifyTier = 6,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyNotification"/> record.
        /// </summary>
        ModifyNotification = 7,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.DismissNotification"/> record.
        /// </summary>
        DismissNotification = 8,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyTenant"/> record.
        /// </summary>
        ModifyTenant = 9,
        /// <summary>
        /// Entry is a <see cref="Jibberwock.DataModels.Security.Audit.EntryTypes.InviteUser"/> record.
        /// </summary>
        InviteUser = 10
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security.Audit
{
    /// <summary>
    /// Describes the type of entry in the audit trail described by an <see cref="AuditTrailEntry"/>.
    /// </summary>
    public enum AuditTrailEntryType
    {
        ControlUserAccess = 1
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.Commands.Auditing
{
    /// <summary>
    /// Return value of <see cref="AuditedCommand{TResult, TDataSource}.Execute"/>.
    /// </summary>
    /// <typeparam name="TResult">The result of executing the command.</typeparam>
    /// <typeparam name="TAuditTrailEntry">The type of audit trail entry to return.</typeparam>
    public class AuditedCommandResult<TResult, TAuditTrailEntry>
        where TAuditTrailEntry : Jibberwock.DataModels.Security.Audit.AuditTrailEntry
    {
        /// <summary>
        /// The result of the command being executed.
        /// </summary>
        public TResult Result { get; private set; }

        /// <summary>
        /// The resultant audit trail entry.
        /// </summary>
        public TAuditTrailEntry AuditTrailEntry { get; private set; }

        internal AuditedCommandResult(TResult result, TAuditTrailEntry auditTrailEntry)
        {
            Result = result;
            AuditTrailEntry = auditTrailEntry;
        }
    }
}

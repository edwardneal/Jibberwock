using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security.Audit
{
    /// <summary>
    /// Represents a single entry in the audit trail.
    /// </summary>
    public abstract class AuditTrailEntry
    {
        /// <summary>
        /// The unique internal reference of this <see cref="AuditTrailEntry"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The date and time that this event occurred.
        /// </summary>
        public DateTimeOffset Occurrence { get; set; }

        /// <summary>
        /// The <see cref="User"/> which the event relates to.
        /// </summary>
        public User RelatedUser { get; set; }

        /// <summary>
        /// The <see cref="Tenant"/> which the event relates to.
        /// </summary>
        public Tenant RelatedTenant { get; set; }

        /// <summary>
        /// The <see cref="User"/> who performed the event.
        /// </summary>
        public User PerformedBy { get; set; }

        /// <summary>
        /// If the event was performed by an API call, the trace ID of that call's connection.
        /// </summary>
        public string OriginatingConnectionId { get; set; }

        /// <summary>
        /// If the event was performed by an API call, the service containing that API call.
        /// </summary>
        public Service OriginatingService { get; set; }

        /// <summary>
        /// The type of event which this <see cref="AuditTrailEntry"/> refers to.
        /// </summary>
        public AuditTrailEntryType Type { get; set; }

        /// <summary>
        /// An optional user-defined comment describing this the reason for this action.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Additional metadata which is serialised to the database.
        /// </summary>
        public abstract string Metadata { get; set; }
    }
}

using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Security.Audit;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Gets audit trail entries, filtering by a number of criteria.
    /// </summary>
    public class GetAuditTrail : ValidatingCommand<IEnumerable<AuditTrailEntry>, IReadableDataSource>
    {
        private class ImplAuditTrailEntry : AuditTrailEntry
        { public override string Metadata { get; set; } }

        /// <summary>
        /// The start of the time window to retrieve.
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// The end of the time window to retrieve.
        /// </summary>
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// Filter to audit trail entries relating to this user.
        /// </summary>
        public User AssociatedUser { get; set; }

        /// <summary>
        /// Filter to audit trail entries relating to this tenant.
        /// </summary>
        public Tenant AssociatedTenant { get; set; }

        /// <summary>
        /// Filter to audit trail entries performed by this user.
        /// </summary>
        public User PerformedBy { get; set; }

        /// <summary>
        /// Filter to this type of audit trail entry.
        /// </summary>
        public AuditTrailEntryType? Type { get; set; }

        public GetAuditTrail(ILogger logger, DateTimeOffset? start, DateTimeOffset? end,
            User associatedUser, Tenant associatedTenant,
            User performedBy, AuditTrailEntryType? type)
            : base(logger)
        {
            StartTime = start;
            EndTime = end;
            AssociatedUser = associatedUser;
            AssociatedTenant = associatedTenant;
            PerformedBy = performedBy;
            Type = type;
        }

        protected override async Task<IEnumerable<AuditTrailEntry>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            // This technically isn't using some of our inheritence hierarchy, but it doesn't make much difference at this point
            var auditTrailEntries = await databaseConnection.QueryAsync<ImplAuditTrailEntry, User, Tenant, User, Service, ImplAuditTrailEntry>("security.usp_GetAuditTrail",
                (aud, rU, rT, pbU, svc) =>
                {
                    aud.RelatedUser = rU;
                    aud.RelatedTenant = rT;
                    aud.PerformedBy = pbU;
                    aud.OriginatingService = svc;

                    return aud;
                }, new
                {
                    Start_Time = StartTime,
                    End_Time = EndTime,
                    User_ID = AssociatedUser?.Id,
                    Tenant_ID = AssociatedTenant?.Id,
                    Performed_By_User_ID = PerformedBy?.Id,
                    Type_ID = Type
                },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return auditTrailEntries;
        }
    }
}

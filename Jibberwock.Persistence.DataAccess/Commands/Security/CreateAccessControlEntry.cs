using Dapper;
using Jibberwock.DataModels.Security;
using Jibberwock.DataModels.Security.Audit.EntryTypes;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.Commands.Auditing;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Creates a single access control entry.
    /// </summary>
    public class CreateAccessControlEntry : AuditingCommand<AccessControlEntry, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyAccessControlEntry>
    {
        /// <summary>
        /// The desired access control entry.
        /// </summary>
        [Required]
        public AccessControlEntry AccessControlEntry { get; set; }

        public CreateAccessControlEntry(ILogger logger, User performedBy, string connectionId, long serviceId, string comment, AccessControlEntry accessControlEntry)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            AccessControlEntry = accessControlEntry;
        }

        protected override async Task<AccessControlEntry> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyAccessControlEntry provisionalAuditTrailEntry)
        {
            if (AccessControlEntry.Group == null)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group must have a value");
            if (AccessControlEntry.Group.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group.Id must have a value");
            if (AccessControlEntry.Group.Tenant == null)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group.Tenant must have a value");
            if (AccessControlEntry.Group.Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group.Tenant.Id must have a value");
            if (AccessControlEntry.Resource == null)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Resource must have a value");
            if (AccessControlEntry.Resource.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Resource.Id must have a value");
            if (!Enum.IsDefined(typeof(Permission), AccessControlEntry.Permission))
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Permission must be a valid Permission");

            var databaseConnection = await dataSource.GetDbConnection();

            provisionalAuditTrailEntry.RelatedTenant = AccessControlEntry.Group.Tenant;

            var resultantAccessControlEntry = await databaseConnection.QueryAsync<AccessControlEntry, Group, dynamic, Tenant, AccessControlEntry>("security.usp_CreateAccessControlEntry",
                (ace, grp, sr, ten) =>
                {
                    if (ace != null && grp != null)
                    { ace.Group = grp; }

                    if (ace != null && sr != null)
                    { ace.Resource = SecurableResourceHelpers.GetSecurableResourceFromDatabase(sr); }

                    if (ace != null && ten != null)
                    { ace.ResourceTenant = ten; }

                    return ace;
                },
                new
                {
                    Tenant_ID = AccessControlEntry.Group.Tenant.Id,
                    User_ID = PerformedBy.Id,
                    Security_Group_ID = AccessControlEntry.Group.Id,
                    Securable_Resource_ID = AccessControlEntry.Resource.Id,
                    Permission_ID = AccessControlEntry.Permission
                },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            AccessControlEntry = resultantAccessControlEntry.FirstOrDefault();

            provisionalAuditTrailEntry.AccessControlEntry = AccessControlEntry;
            provisionalAuditTrailEntry.NewAccessControlEntry = true;

            return AccessControlEntry;
        }
    }
}

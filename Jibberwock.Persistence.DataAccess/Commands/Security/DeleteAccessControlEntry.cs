using Dapper;
using Jibberwock.DataModels.Security;
using Jibberwock.DataModels.Security.Audit.EntryTypes;
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
    /// Removes an access control entry.
    /// </summary>
    public class DeleteAccessControlEntry : AuditingCommand<bool, Jibberwock.DataModels.Security.Audit.EntryTypes.DeleteAccessControlEntry>
    {
        /// <summary>
        /// The access control entry to remove.
        /// </summary>
        [Required]
        public AccessControlEntry AccessControlEntry { get; set; }

        public DeleteAccessControlEntry(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, AccessControlEntry accessControlEntry)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            AccessControlEntry = accessControlEntry;
        }

        protected override async Task<bool> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.DeleteAccessControlEntry provisionalAuditTrailEntry)
        {
            if (AccessControlEntry.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Id must have a value");
            if (AccessControlEntry.Group == null)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group must have a value");
            if (AccessControlEntry.Group.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group.Id must have a value");
            if (AccessControlEntry.Group.Tenant == null)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group.Tenant must have a value");
            if (AccessControlEntry.Group.Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(AccessControlEntry), "AccessControlEntry.Group.Tenant.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            provisionalAuditTrailEntry.RelatedTenant = AccessControlEntry.Group.Tenant;

            var successfullyDeleted = await databaseConnection.ExecuteScalarAsync<bool>("security.usp_DeleteAccessControlEntry",
                new { Tenant_ID = AccessControlEntry.Group.Tenant.Id, Security_Group_ID = AccessControlEntry.Group.Id, Access_Control_Entry_ID = AccessControlEntry.Id },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            provisionalAuditTrailEntry.AccessControlEntry = AccessControlEntry;

            return successfullyDeleted;
        }
    }
}

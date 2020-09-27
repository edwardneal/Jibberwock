using Dapper;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.TableTypes.Security;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Checks whether or not the current user has all of the specified permissions over the specified securable resources.
    /// </summary>
    public class CheckPermissions : ValidatingCommand<bool, IReadableDataSource>
    {
        /// <summary>
        /// The user to check permissions of.
        /// </summary>
        [Required]
        public User User { get; set; }

        /// <summary>
        /// All of the relevant permission checks to run.
        /// </summary>
        [Required]
        [MinLength(1)]
        public IEnumerable<ResourcePermissionCheck> PermissionChecks { get; set; }

        public CheckPermissions(ILogger logger, User user, IEnumerable<ResourcePermissionCheck> permissionChecks)
            : base(logger)
        {
            User = user;
            PermissionChecks = permissionChecks;
        }

        protected override async Task<bool> OnExecute(IReadableDataSource dataSource)
        {
            // A User Id of zero is anonymous and has no permissions
            if (User.Id == 0)
                return false;

            var databaseConnection = await dataSource.GetDbConnection();
            var permissionListRecords = (from pc in PermissionChecks
                                         from perm in pc.PermissionsRequired
                                         select new SecurableResourcePermissionCheck((long)pc.ResourceId, pc.ResourceType, perm))
                                         .AsTableValuedParameter("security.udt_SecurableResourcePermissionCheck");

            var hasPermissions = await databaseConnection.ExecuteScalarAsync<bool>("security.usp_CheckUserPermissions",
                new { User_ID = User.Id, Permission_Checks = permissionListRecords },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return hasPermissions;
        }
    }
}

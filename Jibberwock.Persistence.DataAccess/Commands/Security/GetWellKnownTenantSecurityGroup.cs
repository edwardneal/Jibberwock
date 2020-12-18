using Dapper;
using Jibberwock.DataModels.Security;
using Jibberwock.DataModels.Tenants;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Security
{
    /// <summary>
    /// Gets the details, members and permissions of a well-known security group for a specific tenant.
    /// </summary>
    public class GetWellKnownTenantSecurityGroup : ValidatingCommand<Group, IReadableDataSource>
    {
        /// <summary>
        /// The tenant to query.
        /// </summary>
        [Required]
        public Tenant Tenant { get; set; }

        /// <summary>
        /// The well-known group type to retrieve.
        /// </summary>
        [Required]
        public WellKnownGroupType WellKnownGroupType { get; set; }

        public GetWellKnownTenantSecurityGroup(ILogger logger, Tenant tenant, WellKnownGroupType wellKnownGroupType)
            : base(logger)
        {
            Tenant = tenant;
            WellKnownGroupType = wellKnownGroupType;
        }

        protected override async Task<Group> OnExecute(IReadableDataSource dataSource)
        {
            if (Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tenant), "Tenant.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var groupDetailsBatch = await databaseConnection.QueryMultipleAsync("security.usp_GetWellKnownTenantSecurityGroup",
                new { Tenant_ID = Tenant.Id, Well_Known_Group_Type_ID = WellKnownGroupType },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            var groupDetails = await groupDetailsBatch.ReadSingleOrDefaultAsync<Group>();
            var memberDetails = await groupDetailsBatch.ReadAsync();
            var permissionDetails = await groupDetailsBatch.ReadAsync();

            groupDetails.Users = (from member in memberDetails
                                  select new GroupMembership()
                                  {
                                      Id = member.Id,
                                      Enabled = member.Enabled,
                                      User = new Jibberwock.DataModels.Users.User()
                                      {
                                          Id = member.UserId,
                                          Name = member.UserName
                                      }
                                  }).ToArray();
            groupDetails.AccessControlEntries = (from ace in permissionDetails
                                                 select new AccessControlEntry()
                                                 {
                                                     Id = ace.Id,
                                                     Permission = (Permission)ace.Permission,
                                                     Resource = SecurableResourceHelpers.GetSecurableResourceFromDatabase(ace)
                                                 }).ToArray();

            return groupDetails;
        }
    }
}

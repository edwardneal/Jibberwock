using Dapper;
using Jibberwock.DataModels.Security;
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
    /// Gets the details, members and permissions of a specific group.
    /// </summary>
    public class GetSecurityGroupById : ValidatingCommand<Group, IReadableDataSource>
    {
        /// <summary>
        /// The security group to query for.
        /// </summary>
        [Required]
        public Group SecurityGroup { get; set; }

        public GetSecurityGroupById(ILogger logger, Group securityGroup)
            : base(logger)
        {
            SecurityGroup = securityGroup;
        }

        protected override async Task<Group> OnExecute(IReadableDataSource dataSource)
        {
            if (SecurityGroup.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(SecurityGroup), "SecurityGroup.Id must have a value");
            if (SecurityGroup.Tenant == null)
                throw new ArgumentOutOfRangeException(nameof(SecurityGroup), "SecurityGroup.Tenant must have a value");
            if (SecurityGroup.Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(SecurityGroup), "SecurityGroup.Tenant.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var groupDetailsBatch = await databaseConnection.QueryMultipleAsync("security.usp_GetSecurityGroupById",
                new { Tenant_ID = SecurityGroup.Tenant.Id, Security_Group_ID = SecurityGroup.Id },
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
                                                     Resource = getSecurableResource(ace)
                                                 }).ToArray();

            return groupDetails;
        }

        private static SecurableResource getSecurableResource(dynamic resourceRow)
        {
            var resourceType = (SecurableResourceType)resourceRow.ResourceType;

            switch (resourceType)
            {
                case SecurableResourceType.Tenant:
                    return new Jibberwock.DataModels.Tenants.Tenant() { Id = resourceRow.Id, Name = resourceRow.Name, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.ApiKey:
                    throw new ArgumentOutOfRangeException("ResourceType");
                case SecurableResourceType.Product:
                    return new Jibberwock.DataModels.Products.Product() { Id = resourceRow.Id, Name = resourceRow.Name, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.Service:
                    return new Jibberwock.DataModels.Core.Service() { Id = resourceRow.Id, Name = resourceRow.Name, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.Allert_AlertDefinition:
                    return new Jibberwock.DataModels.Allert.AlertDefinition() { Id = resourceRow.Id, Name = resourceRow.Name, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.Allert_AlertDefinitionGroup:
                    return new Jibberwock.DataModels.Allert.AlertDefinitionGroup() { Id = resourceRow.Id, Name = resourceRow.Name, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                default:
                    throw new ArgumentOutOfRangeException("ResourceType");
            }
        }
    }
}

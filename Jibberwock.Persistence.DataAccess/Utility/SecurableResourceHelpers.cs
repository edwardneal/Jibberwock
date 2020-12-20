using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.Utility
{
    internal class SecurableResourceHelpers
    {
        public static SecurableResource GetSecurableResourceFromDatabase(dynamic resourceRow)
        {
            var resourceType = (SecurableResourceType)resourceRow.ResourceType;

            switch (resourceType)
            {
                case SecurableResourceType.Tenant:
                    return new Jibberwock.DataModels.Tenants.Tenant() { Id = resourceRow.ResourceId, Name = resourceRow.ResourceName, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.ApiKey:
                    throw new ArgumentOutOfRangeException("ResourceType");
                case SecurableResourceType.Product:
                    return new Jibberwock.DataModels.Products.Product() { Id = resourceRow.ResourceId, Name = resourceRow.ResourceName, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.Service:
                    return new Jibberwock.DataModels.Core.Service() { Id = resourceRow.ResourceId, Name = resourceRow.ResourceName, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.Allert_AlertDefinition:
                    return new Jibberwock.DataModels.Allert.AlertDefinition() { Id = resourceRow.ResourceId, Name = resourceRow.ResourceName, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                case SecurableResourceType.Allert_AlertDefinitionGroup:
                    return new Jibberwock.DataModels.Allert.AlertDefinitionGroup() { Id = resourceRow.ResourceId, Name = resourceRow.ResourceName, ResourceIdentifier = resourceRow.ResourceIdentifier, ResourceType = resourceType };
                default:
                    throw new ArgumentOutOfRangeException("ResourceType");
            }
        }
    }
}

using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.TableTypes.Security
{
    internal class SecurableResourcePermissionCheck : UserDefinedTableType
    {
        public long ResourceId { get; set; }

        public SecurableResourceType ResourceType { get; set; }

        public Permission Permission { get; set; }

        public SecurableResourcePermissionCheck(long resourceId, SecurableResourceType resourceType, Permission permission)
            : base(
                  GetColumnMetadata<long>("Securable_Resource_ID"),
                  GetColumnMetadata<SecurableResourceType>("Securable_Resource_Type_ID"),
                  GetColumnMetadata<Permission>("Permission_ID")
                  )
        {
            base.SetValue(0, resourceId);
            base.SetValue(1, resourceType);
            base.SetValue(2, permission);

            ResourceId = resourceId;
            ResourceType = resourceType;
            Permission = permission;
        }
    }
}

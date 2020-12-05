using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.TableTypes.Products
{
    internal class Subscription : UserDefinedTableType
    {
        public long SubscriptionId { get; set; }

        public Subscription(long subscriptionId)
            : base(GetColumnMetadata<long>("Subscription_ID"))
        {
            base.SetValue<long>(0, subscriptionId);

            SubscriptionId = subscriptionId;
        }
    }
}

using Jibberwock.DataModels.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Security
{
    /// <summary>
    /// This grants <see cref="Permission"/>s over a securable object to a specific <see cref="Group"/>
    /// </summary>
    public class AccessControlEntry
    {
        /// <summary>
        /// The unique internal reference for this <see cref="AccessControlEntry"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The <see cref="Group"/> which should be given these <see cref="Permission"/>s
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// The <see cref="Permission"/>s which the <see cref="Group"/> should be granted over the securable object
        /// </summary>
        public Permission Permission { get; set; }

        /// <summary>
        /// The resource which this <see cref="AccessControlEntry"/> applies to.
        /// </summary>
        public SecurableResource Resource { get; set; }

        /// <summary>
        /// The <see cref="Tenant"/> which the <see cref="Resource"/> property is a member of.
        /// </summary>
        public Tenant ResourceTenant { get; set; }
    }
}

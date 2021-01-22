using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert
{
    /// <summary>
    /// Represents a group of <see cref="AlertDefinition"/>s which process input data.
    /// </summary>
    public class AlertDefinitionGroup : SecurableResource
    {
        /// <summary>
        /// The friendly name of this <see cref="AlertDefinitionGroup"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All <see cref="AlertDefinition"/>s which this <see cref="AlertDefinitionGroup"/> contains, with their Enabled statuses.
        /// </summary>
        public IEnumerable<AlertDefinitionGroupMembership> GroupMemberships { get; set; }

        /// <summary>
        /// Any extra metadata to be associated with this <see cref="AlertDefinitionGroup"/>.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The child <see cref="AlertDefinitionGroup"/>s of this <see cref="AlertDefinitionGroup"/>.
        /// </summary>
        public AlertDefinitionGroup ChildGroups { get; set; }
    }
}

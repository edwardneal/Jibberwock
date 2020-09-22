using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert
{
    /// <summary>
    /// Cross-references an <see cref="AlertDefinition"/> to an <see cref="AlertDefinitionGroup"/>.
    /// </summary>
    public class AlertDefinitionGroupMembership
    {
        /// <summary>
        /// The unique internal reference for this <see cref="AlertDefinitionGroupMembership"/>.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Determines whether or not this <see cref="AlertDefinitionGroupMembership"/> takes effect.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The <see cref="AlertDefinition"/> which <see cref="AlertDefinitionGroup"/> contains.
        /// </summary>
        public AlertDefinition AlertDefinition { get; set; }

        /// <summary>
        /// The <see cref="AlertDefinitionGroup"/> which <see cref="AlertDefinition"/> is a member of.
        /// </summary>
        public AlertDefinitionGroup AlertDefinitionGroup { get; set; }
    }
}

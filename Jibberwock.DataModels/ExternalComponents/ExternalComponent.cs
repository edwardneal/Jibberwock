using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.ExternalComponents
{
    /// <summary>
    /// A single external component, optionally with a programatically-updated status.
    /// </summary>
    public class ExternalComponent
    {
        /// <summary>
        /// The unique internal reference for this <see cref="ExternalComponent"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The reference of this component within the third-party service.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The purpose this component fulfils.
        /// </summary>
        public Purpose Purpose { get; set; }

        /// <summary>
        /// The current status of this component.
        /// </summary>
        public Status.ExternalComponentStatus Status { get; set; }
    }
}

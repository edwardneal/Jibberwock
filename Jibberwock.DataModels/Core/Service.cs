using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// Represents a single service (such as the Jibberwock Admin portal or the Allert service.)
    /// </summary>
    public class Service : SecurableResource
    {
        /// <summary>
        /// The friendly name of this <see cref="Service"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The primary web address of this <see cref="Service"/>.
        /// </summary>
        public string Url { get; set; }
    }
}

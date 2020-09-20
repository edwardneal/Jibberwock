using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.ExternalComponents
{
    /// <summary>
    /// This class describes the purpose of a third-party component (such as email sending, etc.)
    /// </summary>
    public class Purpose
    {
        /// <summary>
        /// The unique internal reference for this <see cref="Purpose"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of this purpose.
        /// </summary>
        public string Name { get; set; }
    }
}

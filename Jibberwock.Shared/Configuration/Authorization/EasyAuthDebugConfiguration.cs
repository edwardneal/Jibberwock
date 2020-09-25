using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Configuration.Authorization
{
    /// <summary>
    /// Contains the configuration elements to emulate Azure App Services authentication (aka EasyAuth.)
    /// </summary>
    public class EasyAuthDebugConfiguration
    {
        /// <summary>
        /// The new value for the X-MS-CLIENT-PRINCIPAL-IDP header.
        /// </summary>
        public string IdProvider { get; set; }

        /// <summary>
        /// The new value for the X-MS-CLIENT-PRINCIPAL header.
        /// </summary>
        public string Principal { get; set; }
    }
}

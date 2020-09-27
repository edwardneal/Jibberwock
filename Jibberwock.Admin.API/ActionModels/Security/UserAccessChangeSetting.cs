using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Security
{
    /// <summary>
    /// Describes the settings sent to <see cref="Jibberwock.Admin.API.Controllers.Security.UserController.ControlUserAccess(long, UserAccessChangeSetting)"/>.
    /// </summary>
    public class UserAccessChangeSetting
    {
        /// <summary>
        /// The enabled state of the user. If <c>false</c>, the user will be treated as if they are unauthenticated.
        /// </summary>
        public bool Enabled { get; set; }
    }
}

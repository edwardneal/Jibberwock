using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Notifications
{
    /// <summary>
    /// Describes the data sent to <see cref="Jibberwock.Admin.API.Controllers.Security.UserController.NotifyAllUsers(NotifyRequest)"/>,
    /// <see cref="Jibberwock.Admin.API.Controllers.Security.UserController.NotifyUser(long, NotifyRequest)"/>,
    /// <see cref="Jibberwock.Admin.API.Controllers.Security.UserController.UpdateNotification(long, long, NotifyRequest)"/>,
    /// <see cref="Jibberwock.Admin.API.Controllers.Security.UserController.UpdateGlobalNotification(long, NotifyRequest)"/>,
    /// <see cref="Jibberwock.Admin.API.Controllers.Tenants.TenantController.NotifyTenant(long, NotifyRequest)"/> and
    /// <see cref="Jibberwock.Admin.API.Controllers.Tenants.TenantController.UpdateNotification(long, long, NotifyRequest)"/>
    /// in order to notify <see cref="User"/>s or <see cref="Tenant"/>s.
    /// </summary>
    public class NotifyRequest
    {
        /// <summary>
        /// The current status of the <see cref="Notification"/> to create or update.
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        /// The type of <see cref="Notification"/> to create or update.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// The priority of the <see cref="Notification"/> to create or update.
        /// </summary>
        public NotificationPriority Priority { get; set; }

        /// <summary>
        /// The first date that the created or updated <see cref="Notification"/> should be visible.
        /// </summary>
        /// <remarks>
        /// If the <see cref="Notification"/> is sent as an email, this cannot be changed.
        /// </remarks>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// The last date that the created or updated <see cref="Notification"/> should be visible.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// A short summary of the created or updated <see cref="Notification"/>.
        /// </summary>
        /// <remarks>
        /// This cannot be changed on a <see cref="Notification"/> which is sent as an email.
        /// </remarks>
        public string Subject { get; set; }

        /// <summary>
        /// The full content of the created or updated <see cref="Notification"/>.
        /// </summary>
        /// <remarks>
        /// This cannot be changed on a <see cref="Notification"/> which is sent as an email.
        /// </remarks>
        public string Message { get; set; }

        /// <summary>
        /// Determines whether or not the users are able to dismiss this message from the front-ends.
        /// </summary>
        public bool AllowDismissal { get; set; }

        /// <summary>
        /// Determines whether or not to send the created or updated <see cref="Notification"/> as an email.
        /// </summary>
        /// <remarks>
        /// This cannot be changed if the <see cref="Notification"/>'s start date has passed.
        /// </remarks>
        public bool SendAsEmail { get; set; }
    }
}

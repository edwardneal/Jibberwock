using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// Represents a one-way notification sent to a user or tenant.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// The unique internal identifier for this <see cref="Notification"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// If not null, the <see cref="User"/> which this <see cref="Notification"/> should appear for.
        /// </summary>
        public User TargetUser { get; set; }

        /// <summary>
        /// If not null, all <see cref="User"/>s which are a member of this <see cref="Tenant"/> will receive this <see cref="Notification"/>.
        /// </summary>
        public Tenant TargetTenant { get; set; }

        /// <summary>
        /// If not null, the email batch which should send this <see cref="Notification"/>.
        /// </summary>
        public EmailBatch EmailBatch { get; set; }

        /// <summary>
        /// The current status of this <see cref="Notification"/>, indicating whether it has been cancelled.
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        /// The type of this <see cref="Notification"/>. This may change the way this <see cref="Notification"/> is displayed to the users.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// The priority of this <see cref="Notification"/>.
        /// </summary>
        public NotificationPriority Priority { get; set; }

        /// <summary>
        /// The date that this <see cref="Notification"/> was created.
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// The first date that this <see cref="Notification"/> should become visible (or be sent to users.)
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// The last date that this <see cref="Notification"/> should be visible.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// A short description of this <see cref="Notification"/>, or the subject of an email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The full content of this notification.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// If <c>false</c>, the user is unable to dismiss this message from the front-end.
        /// </summary>
        public bool AllowDismissal { get; set; }
    }
}

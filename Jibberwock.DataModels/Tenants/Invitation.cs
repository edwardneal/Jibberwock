using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Tenants
{
    /// <summary>
    /// An <see cref="Invitation"/> represents a <see cref="User"/> which has been invited to a <see cref="Tenant"/>.
    /// </summary>
    public class Invitation
    {
        /// <summary>
        /// The unique internal reference for this <see cref="Invitation"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// If an email was sent, the batch which creates the email.
        /// </summary>
        public EmailBatch EmailBatch { get; set; }

        /// <summary>
        /// The timestamp when the <see cref="User"/> was invited.
        /// </summary>
        public DateTimeOffset InvitationDate { get; set; }

        /// <summary>
        /// This <see cref="Invitation"/> will not be acceptable after this date.
        /// </summary>
        public DateTimeOffset? ExpirationDate { get; set; }

        /// <summary>
        /// The <see cref="Tenant"/> being invited to.
        /// </summary>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// The <see cref="User"/> being invited.
        /// </summary>
        /// <remarks>
        /// If a record with the invited email address and identity provider already exists, <see cref="InvitedUser"/> will refer to this record.
        /// If not, <see cref="InvitedUser"/> will refer to a "virtual user", which will be linked to any required security group memberships within the <see cref="Tenant"/>.
        /// </remarks>
        public User InvitedUser { get; set; }

        /// <summary>
        /// The email address of the person being invited.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The external identity provider of the person being invited.
        /// </summary>
        public string ExternalIdentityProvider { get; set; }

        /// <summary>
        /// The current status of this <see cref="Invitation"/>.
        /// </summary>
        public InvitationStatus Status { get; set; }
    }
}

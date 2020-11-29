using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Security.Audit.EntryTypes;
using Jibberwock.DataModels.Tenants;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.Commands.Auditing;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Tenants
{
    public class Invite : AuditingCommand<Invitation, InviteUser>
    {
        private readonly string _baseUrl;
        private readonly IQueueDataSource _queueDataSource;
        private readonly string _emailQueueName;

        /// <summary>
        /// The invitation to send (including target.)
        /// </summary>
        [Required]
        public Invitation Invitation { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="Invitation"/> will also be sent as an email.
        /// </summary>
        [Required]
        public bool SendAsEmail { get; set; }

        public Invite(ILogger logger, User performedBy, string connectionId, int serviceId, string comment,
            Invitation invitation, bool sendAsEmail, IQueueDataSource queueDataSource, string emailQueueName,
            string baseUrl)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            _baseUrl = baseUrl;
            _queueDataSource = queueDataSource ?? throw new ArgumentNullException(nameof(queueDataSource));
            _emailQueueName = emailQueueName ?? throw new ArgumentNullException(nameof(emailQueueName));

            Invitation = invitation;
            SendAsEmail = sendAsEmail;
        }

        protected override async Task<Invitation> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, InviteUser provisionalAuditTrailEntry)
        {
            if (Invitation.Tenant == null)
                throw new ArgumentNullException(nameof(Invitation), "Invitation.Tenant must have a value.");
            if (Invitation.Tenant.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Invitation), "Invitation.Tenant.Id must have a value.");
            if (string.IsNullOrWhiteSpace(Invitation.EmailAddress))
                throw new ArgumentNullException(nameof(Invitation), "Invitation.EmailAddress must have a value.");
            if (Invitation.EmailAddress.Length > 256)
                throw new ArgumentOutOfRangeException(nameof(Invitation), "Invitation.EmailAddress must be less than or equal to 256 characters long.");
            if (string.IsNullOrWhiteSpace(Invitation.ExternalIdentityProvider))
                throw new ArgumentNullException(nameof(Invitation), "Invitation.ExternalIdentityProvider must have a value.");
            if (Invitation.ExternalIdentityProvider.Length > 32)
                throw new ArgumentOutOfRangeException(nameof(Invitation), "Invitation.ExternalIdentityProvider must be less than or equal to 32 characters long.");

            var databaseConnection = await dataSource.GetDbConnection();
            // This is a multi-step approach. We create the record in the database, then generate
            // a message using the QueueClient, then return
            var createdInvitations = await databaseConnection.QueryAsync<Invitation, Tenant, EmailBatch, Invitation>("tenants.usp_CreateInvitation",
                (inv, ten, eb) =>
                {
                    if (ten != null && ten.Id != 0)
                    { inv.Tenant = ten; }

                    if (eb != null && eb.Id != 0)
                    { inv.EmailBatch = eb; }
                    return inv;
                }, new
                {
                    Tenant_ID = Invitation.Tenant.Id,
                    Email_Address = Invitation.EmailAddress,
                    Identity_Provider = Invitation.ExternalIdentityProvider,
                    Expiration_Date = Invitation.ExpirationDate,
                    Send_As_Email = SendAsEmail
                }, commandType: CommandType.StoredProcedure, transaction: transaction, commandTimeout: 30);
            var resultantInvitation = createdInvitations.FirstOrDefault();

            Invitation = resultantInvitation;

            provisionalAuditTrailEntry.EmailBatch = Invitation.EmailBatch;
            provisionalAuditTrailEntry.RelatedUser = Invitation.InvitedUser;
            provisionalAuditTrailEntry.RelatedTenant = Invitation.Tenant;

            return Invitation;
        }

        protected override async Task OnCommandCompleted(InviteUser auditTrailEntry, Invitation result)
        {
            var emailQueueClient = SendAsEmail ? _queueDataSource.GetQueueClient(_emailQueueName) : null;

            if (result.EmailBatch != null)
            {
                // If we've got an email batch, then we need to put the message onto the queue!
                var messageToCreate = ServiceBusUtilities.GenerateMessage(new { Metadata = new { baseUrl = _baseUrl } }, result.EmailBatch.ServiceBusMessageId);

                await emailQueueClient.SendAsync(messageToCreate);
                // Don't need to expose this to the clients though
                Invitation.EmailBatch.ServiceBusMessageId = null;
            }
        }
    }
}

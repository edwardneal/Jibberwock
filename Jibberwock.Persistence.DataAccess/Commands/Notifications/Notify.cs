﻿using Dapper;
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
using System.Threading;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Notifications
{
    /// <summary>
    /// Notifies a user, a tenant or all users, optionally sending this notification as an email.
    /// </summary>
    public class Notify : AuditingCommand<Notification, ModifyNotification>
    {
        private readonly IQueueDataSource _queueDataSource;
        private readonly string _emailQueueName;

        /// <summary>
        /// The notification to send (including target.)
        /// </summary>
        [Required]
        public Notification Notification { get; set; }

        /// <summary>
        /// If <c>true</c>, this <see cref="Notification"/> will also be sent as an email.
        /// </summary>
        [Required]
        public bool SendAsEmail { get; set; }

        public Notify(ILogger logger, User performedBy, string connectionId, long serviceId, string comment,
            Notification notification, bool sendAsEmail, IQueueDataSource queueDataSource, string emailQueueName)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            _queueDataSource = queueDataSource ?? throw new ArgumentNullException(nameof(queueDataSource));
            _emailQueueName = emailQueueName ?? throw new ArgumentNullException(nameof(emailQueueName));

            Notification = notification;
            SendAsEmail = sendAsEmail;
        }

        protected override async Task<Notification> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyNotification provisionalAuditTrailEntry)
        {
            if (Notification.Priority == null)
                throw new ArgumentNullException(nameof(Notification), "Notification.Priority must have a value.");
            if (string.IsNullOrWhiteSpace(Notification.Priority.Name))
                throw new ArgumentNullException(nameof(Notification), "Notification.Priority.Name must have a value.");
            if (Notification.Priority.Name.Length > 32)
                throw new ArgumentOutOfRangeException(nameof(Notification), "Notification.Priority.Name must be less than or equal to 32 characters long.");
            if (string.IsNullOrWhiteSpace(Notification.Subject))
                throw new ArgumentNullException(nameof(Notification), "Notification.Subject must have a value.");
            if (Notification.Subject.Length > 128)
                throw new ArgumentOutOfRangeException(nameof(Notification), "Notification.Subject must be less than or equal to 128 characters long.");
            if (string.IsNullOrWhiteSpace(Notification.Message))
                throw new ArgumentNullException(nameof(Notification), "Notification.Message must have a value.");

            if ((Notification.TargetUser != null && Notification.TargetUser.Id != 0)
                && (Notification.TargetTenant != null && Notification.TargetTenant.Id != 0))
                throw new ArgumentOutOfRangeException(nameof(Notification), "A notification cannot target both a tenant and a user,"); ;

            var databaseConnection = await dataSource.GetDbConnection();
            // This is a multi-step approach. We create the record in the database, then generate
            // a message using the QueueClient, then return
            var createdNotifications = await databaseConnection.QueryAsync<Notification, NotificationPriority, User, Tenant, EmailBatch, Notification>("core.usp_CreateNotification",
                (n, np, usr, ten, eb) =>
                {
                    n.Priority = np;

                    if (usr != null && usr.Id != 0)
                    { n.TargetUser = usr; }

                    if (ten != null && ten.Id != 0)
                    { n.TargetTenant = ten; }

                    if (eb != null && eb.Id != 0)
                    { n.EmailBatch = eb; }
                    return n;
                }, new
                {
                    User_ID = Notification.TargetUser?.Id,
                    Tenant_ID = Notification.TargetTenant?.Id,
                    Status_ID = Notification.Status,
                    Type_ID = Notification.Type,
                    Priority_Name = Notification.Priority?.Name,
                    Start_Date = Notification.StartDate,
                    End_Date = Notification.EndDate,
                    Subject = Notification.Subject,
                    Message = Notification.Message,
                    Allow_Dismissal = Notification.AllowDismissal,
                    Send_As_Email = SendAsEmail
                }, commandType: CommandType.StoredProcedure, transaction: transaction, commandTimeout: 30);
            var resultantNotification = createdNotifications.FirstOrDefault();

            Notification = resultantNotification;
            provisionalAuditTrailEntry.ServiceBusMessageId = Notification.EmailBatch?.ServiceBusMessageId;

            provisionalAuditTrailEntry.Notification = Notification;
            provisionalAuditTrailEntry.RelatedUser = Notification.TargetUser;
            provisionalAuditTrailEntry.RelatedTenant = Notification.TargetTenant;
            provisionalAuditTrailEntry.NewNotification = true;
            provisionalAuditTrailEntry.SendAsEmail = SendAsEmail;

            return Notification;
        }

        protected override async Task OnCommandCompleted(IReadWriteDataSource dataSource, ModifyNotification auditTrailEntry, Notification result)
        {
            var emailQueueClient = SendAsEmail ? _queueDataSource.GetQueueClient(_emailQueueName) : null;

            if (result.EmailBatch != null)
            {
                // If we've got an email batch, then we need to put the message onto the queue!
                var messageToCreate = ServiceBusUtilities.GenerateMessage(new { Metadata = default(object) }, result.EmailBatch.ServiceBusMessageId, result.StartDate);

                await emailQueueClient.SendAsync(messageToCreate);
                // Don't need to expose this to the clients though
                Notification.EmailBatch.ServiceBusMessageId = null;
            }
        }
    }
}

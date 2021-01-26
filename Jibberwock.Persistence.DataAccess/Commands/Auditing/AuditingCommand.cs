using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Auditing
{
    public abstract class AuditingCommand<TResult, TAuditTrailEntry> : ValidatingCommand<AuditedCommandResult<TResult, TAuditTrailEntry>, IReadWriteDataSource>
        where TAuditTrailEntry : Jibberwock.DataModels.Security.Audit.AuditTrailEntry, new()
    {
        protected AuditingCommand(ILogger logger, User performedBy, string connectionId, long serviceId, string comment)
            : base(logger)
        {
            PerformedBy = performedBy;
            ConnectionId = connectionId;
            OriginatingService = new Service() { Id = serviceId };
            Comment = comment;
        }

        protected AuditingCommand(ILogger logger, int serviceId)
            : this(logger, null, null, serviceId, null)
        {
        }

        /// <summary>
        /// The <see cref="User"/> performing this action.
        /// </summary>
        public User PerformedBy { get; set; }

        /// <summary>
        /// The unique identifier of this connection.
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// The service which performed this action.
        /// </summary>
        public Service OriginatingService { get; set; }

        /// <summary>
        /// An optional user-defined command describing the reason for this action.
        /// </summary>
        public string Comment { get; set; }

        protected abstract Task<TResult> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, TAuditTrailEntry provisionalAuditTrailEntry);

        protected virtual Task OnCommandCompleted(TAuditTrailEntry auditTrailEntry, TResult result)
        {
            return Task.CompletedTask;
        }

        protected override async Task<AuditedCommandResult<TResult, TAuditTrailEntry>> OnExecute(IReadWriteDataSource dataSource)
        {
            // The audit trail entry and the command will execute in the same transaction
            var databaseConnection = await dataSource.GetDbConnection() as DbConnection;
            var dbTransaction = await databaseConnection.BeginTransactionAsync();

            try
            {
                var auditTrailEntry = new TAuditTrailEntry()
                    {
                        Occurrence = DateTimeOffset.Now,
                        PerformedBy = PerformedBy,
                        OriginatingConnectionId = ConnectionId,
                        OriginatingService = OriginatingService,
                        Comment = Comment
                    };
                // Provide a spot for the derived classes to execute their command as required, making any final tweaks to the audit trail entry
                var result = await OnAuditedExecute(dataSource, dbTransaction, auditTrailEntry);

                // Create the audit trail entry itself, updating our ID
                auditTrailEntry.Id = await databaseConnection.ExecuteScalarAsync<long>("security.usp_CreateAuditTrailEntry",
                    new
                    {
                        Audit_Trail_Type_ID = auditTrailEntry.Type,
                        Occurrence_Time = auditTrailEntry.Occurrence,
                        User_ID = auditTrailEntry.RelatedUser?.Id,
                        Tenant_ID = auditTrailEntry.RelatedTenant?.Id,
                        Performed_By_User_ID = auditTrailEntry.PerformedBy?.Id,
                        Originating_Connection_ID = auditTrailEntry.OriginatingConnectionId,
                        Originating_Service_ID = auditTrailEntry.OriginatingService?.Id,
                        Comment = auditTrailEntry.Comment,
                        Metadata = auditTrailEntry.Metadata,
                        Result_Value_Type = 1
                    },
                    transaction: dbTransaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

                // Commit the transaction
                await dbTransaction.CommitAsync();

                // Perform any post-commit activity. This handles situations where some work needs to be performed in an external system (like
                // sending a message to Service Bus) which needs a database record to be created
                await OnCommandCompleted(auditTrailEntry, result);

                return new AuditedCommandResult<TResult, TAuditTrailEntry>(result, auditTrailEntry);
            }
            catch(Exception)
            {
                // If there's an exception, roll the transaction back and rethrow
                await dbTransaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (dbTransaction != null)
                {
                    await dbTransaction.DisposeAsync();
                }
            }
        }
    }
}

using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Emails
{
    /// <summary>
    /// Gets an email batch based upon its external message ID.
    /// </summary>
    public class GetBatch : ValidatingCommand<EmailBatch, IReadableDataSource>
    {
        /// <summary>
        /// The ID of the message in the external message queue.
        /// </summary>
        [StringLength(64, MinimumLength = 1, ErrorMessage = "The external message ID must be between 1 and 64 characters long.")]
        public string ExternalMessageId { get; set; }

        public GetBatch(ILogger logger, string externalMessageId)
            : base(logger)
        {
            ExternalMessageId = externalMessageId;
        }

        protected override async Task<EmailBatch> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var emailBatch = await databaseConnection.QueryAsync<EmailBatch, EmailBatchType, EmailBatch>("core.usp_GetEmailBatchByIdentifier",
                (eb, ebt) =>
                {
                    eb.Type = ebt;
                    return eb;
                }, new { External_Message_ID = ExternalMessageId },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return emailBatch.FirstOrDefault();
        }
    }
}

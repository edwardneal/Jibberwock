using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Emails
{
    public class StartBatch : ValidatingCommand<bool, IReadWriteDataSource>
    {
        /// <summary>
        /// The email batch to start processing.
        /// </summary>
        [Required]
        public EmailBatch EmailBatch { get; set; }

        public StartBatch(ILogger logger, EmailBatch emailBatch)
            : base(logger)
        {
            EmailBatch = emailBatch;
        }

        protected override async Task<bool> OnExecute(IReadWriteDataSource dataSource)
        {
            if (EmailBatch.Id == 0)
                throw new ArgumentNullException(nameof(EmailBatch), "EmailBatch.Id must have a value.");

            var databaseConnection = await dataSource.GetDbConnection();

            var started = await databaseConnection.ExecuteScalarAsync<bool>("core.usp_StartEmailBatch",
                new { Email_Batch_ID = EmailBatch.Id, Date_Started = DateTimeOffset.Now },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return started;
        }
    }
}

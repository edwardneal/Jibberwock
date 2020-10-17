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
    /// Generates all relevant Email records for a specfic email batch.
    /// </summary>
    public class PrepareEmails : ValidatingCommand<(IEnumerable<Email>, IEnumerable<string>), IReadWriteDataSource>
    {
        /// <summary>
        /// The <see cref="EmailBatch"/> which these emails will be associated with.
        /// </summary>
        [Required]
        public EmailBatch EmailBatch { get; set; }

        /// <summary>
        /// All email records to be created.
        /// </summary>
        [MinLength(1, ErrorMessage = "At least one email must be sent in this email batch.")]
        public IEnumerable<Jibberwock.Persistence.DataAccess.TableTypes.Emails.Email> EmailRecords { get; set; }

        public PrepareEmails(ILogger logger, EmailBatch emailBatch, IEnumerable<Jibberwock.Persistence.DataAccess.TableTypes.Emails.Email> emails)
            : base(logger)
        {
            EmailBatch = emailBatch;
            EmailRecords = emails;
        }

        protected override async Task<(IEnumerable<Email>, IEnumerable<string>)> OnExecute(IReadWriteDataSource dataSource)
        {
            if (EmailBatch.Id == 0)
                throw new ArgumentNullException(nameof(EmailBatch), "EmailBatch.Id must have a value.");

            var databaseConnection = await dataSource.GetDbConnection();

            var prepareEmailsReader = await databaseConnection.QueryMultipleAsync("core.usp_StartEmailBatch",
                new { Email_Batch_ID = EmailBatch.Id, Emails_To_Send = EmailRecords },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            var emailExclusions = await prepareEmailsReader.ReadAsync<string>();
            var emailRecordMappings = await prepareEmailsReader.ReadAsync<Email>();

            foreach (var eml in emailRecordMappings)
                eml.SourceBatch = EmailBatch;

            return (emailRecordMappings, emailExclusions);
        }
    }
}

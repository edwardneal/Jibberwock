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
    /// Marks all relevant Email records for a specific email batch as completed.
    /// </summary>
    public class CompleteEmails : ValidatingCommand<bool, IReadWriteDataSource>
    {
        /// <summary>
        /// The <see cref="EmailBatch"/> which these emails must be associated with.
        /// </summary>
        [Required]
        public EmailBatch EmailBatch { get; set; }

        /// <summary>
        /// The date these emails were sent.
        /// </summary>
        [Required]
        public DateTimeOffset DateSent { get; set; }

        /// <summary>
        /// All email records to be marked as complete.
        /// </summary>
        [MinLength(1, ErrorMessage = "At least one email must be marked as complete in this email batch.")]
        public IEnumerable<Jibberwock.Persistence.DataAccess.TableTypes.Emails.Email> EmailRecords { get; set; }

        public CompleteEmails(ILogger logger, EmailBatch emailBatch, DateTimeOffset dateSent, IEnumerable<Jibberwock.Persistence.DataAccess.TableTypes.Emails.Email> emails)
            : base(logger)
        {
            EmailBatch = emailBatch;
            DateSent = dateSent;
            EmailRecords = emails;
        }

        protected override async Task<bool> OnExecute(IReadWriteDataSource dataSource)
        {
            if (EmailBatch.Id == 0)
                throw new ArgumentNullException(nameof(EmailBatch), "EmailBatch.Id must have a value.");

            var databaseConnection = await dataSource.GetDbConnection();

            var completedEmailsSuccessfully = await databaseConnection.ExecuteScalarAsync<bool>("core.usp_CompleteEmails",
                new { Email_Batch_ID = EmailBatch.Id, Date_Sent = DateSent, Sent_Emails = EmailRecords },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return completedEmailsSuccessfully;
        }
    }
}

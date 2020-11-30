using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Emails
{
    /// <summary>
    /// Gets the details of an email batch of type Invitation, based upon its email batch ID.
    /// </summary>
    public class GetInvitationBatch : ValidatingCommand<Invitation, IReadableDataSource>
    {
        /// <summary>
        /// The email batch to get the notification details of.
        /// </summary>
        public EmailBatch EmailBatch { get; set; }

        public GetInvitationBatch(ILogger logger, EmailBatch emailBatch)
            : base(logger)
        {
            EmailBatch = emailBatch;
        }

        protected override async Task<Invitation> OnExecute(IReadableDataSource dataSource)
        {
            if (EmailBatch.Id == 0)
                throw new ArgumentNullException(nameof(EmailBatch), "EmailBatch.Id must have a value.");

            var databaseConnection = await dataSource.GetDbConnection();

            var invitation = await databaseConnection.QueryAsync<Invitation, Tenant, EmailBatch, Invitation>("core.usp_GetInvitationEmailBatchByID",
                (inv, ten, eb) =>
                {
                    if (ten != null && ten.Id != 0)
                        inv.Tenant = ten;
                    if (eb != null && eb.Id != 0)
                        inv.EmailBatch = eb;
                    return inv;
                },
                new { Email_Batch_ID = EmailBatch.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);


            return invitation.FirstOrDefault();
        }
    }
}

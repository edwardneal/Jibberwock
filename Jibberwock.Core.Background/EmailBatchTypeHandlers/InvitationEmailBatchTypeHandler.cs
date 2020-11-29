using Jibberwock.DataModels.Core;
using Jibberwock.DataModels.Tenants;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Shared.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Core.Background.EmailBatchTypeHandlers
{
    public class InvitationEmailBatchTypeHandler : EmailBatchTypeHandlerBase
    {
        private Invitation _invitation;

        public InvitationEmailBatchTypeHandler(ILogger logger, IReadableDataSource dataSource, EmailBatch emailBatch, SendGridConfiguration sendGridConfiguration)
            : base(logger, dataSource, emailBatch, sendGridConfiguration)
        {
        }

        public override IEnumerable<Personalization> GetPersonalizations(dynamic messageMetadata)
        {
            var pers = GetPersonalization();

            pers.TemplateData = new
            {
                tenant = new { name = _invitation.Tenant.Name },
                idp = new { name = _invitation.ExternalIdentityProvider },
                config = new { url = messageMetadata.baseUrl },
                invitation = new { id = _invitation.Id },
                metadata = messageMetadata,
                message_id = pers.CustomArgs[_sendGridConfiguration.EmailIdParameterName]
            };

            pers.Tos = new List<EmailAddress>() { new EmailAddress(_invitation.EmailAddress) };

            yield return pers;
        }

        public override async Task PopulateAsync()
        {
            var getInvitationEmailBatchCommand = new Jibberwock.Persistence.DataAccess.Commands.Emails.GetInvitationBatch(_logger, EmailBatch);
            var batchDetails = await getInvitationEmailBatchCommand.Execute(_dataSource);

            _invitation = batchDetails;
        }
    }
}

using Dapper;
using Jibberwock.DataModels.ExternalComponents;
using Jibberwock.DataModels.ExternalComponents.Status;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.ExternalComponents
{
    /// <summary>
    /// Gets all external components which are monitored by a particular status provider.
    /// </summary>
    public sealed class GetByStatusProvider : ValidatingCommand<IEnumerable<ExternalComponent>, IReadableDataSource>
    {
        /// <summary>
        /// The provider for the components' status.
        /// </summary>
        [Required]
        public StatusProvider StatusProvider { get; set; }

        /// <summary>
        /// Creates an instance of this command, passing the logger to write messages to.
        /// </summary>
        /// <param name="logger">The logger which messages should be written to.</param>
        public GetByStatusProvider(ILogger logger, StatusProvider statusProvider)
            : base(logger)
        {
            StatusProvider = statusProvider;
        }

        protected override async Task<IEnumerable<ExternalComponent>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();
            var results = await databaseConnection.QueryAsync<ExternalComponent, Purpose, ExternalComponentStatus, ExternalComponent>("components.usp_GetByStatusProvider",
                (ec, ecP, ecS) =>
                {
                    ec.Purpose = ecP;
                    ec.Status = ecS;
                    return ec;
                },
                new { Status_Provider_ID = StatusProvider }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return results;
        }
    }
}

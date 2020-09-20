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
    /// Gets all external components which serve a named purpose.
    /// </summary>
    public class GetByPurpose : ValidatingCommand<IEnumerable<ExternalComponent>, IReadableDataSource>
    {
        /// <summary>
        /// The purpose which the external components must serve.
        /// </summary>
        [Required]
        public Purpose Purpose { get; set; }

        /// <summary>
        /// Creates an instance of this command, passing the logger to write messages to.
        /// </summary>
        /// <param name="logger">The logger which messages should be written to.</param>
        public GetByPurpose(ILogger logger, Purpose purpose)
            : base(logger)
        {
            Purpose = purpose;
        }

        protected override async Task<IEnumerable<ExternalComponent>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            if (string.IsNullOrWhiteSpace(Purpose.Name))
                throw new ArgumentException("Purpose name must be specified.");

            var results = await databaseConnection.QueryAsync<ExternalComponent, Purpose, ExternalComponentStatus, ExternalComponent>("components.usp_GetByStatusProvider",
                (ec, ecP, ecS) =>
                {
                    ec.Purpose = ecP;
                    ec.Status = ecS;
                    return ec;
                },
                new { Purpose_Name = Purpose.Name }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return results;
        }
    }
}

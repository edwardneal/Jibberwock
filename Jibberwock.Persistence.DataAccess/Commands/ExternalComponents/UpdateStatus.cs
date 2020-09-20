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
    /// Updates the status of an external component.
    /// </summary>
    public class UpdateStatus : ValidatingCommand<ExternalComponentStatus, IReadWriteDataSource>
    {
        /// <summary>
        /// The external component to change the status of.
        /// </summary>
        [Required]
        public ExternalComponent ExternalComponent { get; set; }

        /// <summary>
        /// The unprocessed status from the status provider.
        /// </summary>
        [Required]
        public string ThirdPartyStatus { get; set; }

        /// <summary>
        /// A raw true/false indicator of whether or not the component is available.
        /// </summary>
        [Required]
        public bool ComponentAvailable { get; set; }

        /// <summary>
        /// Creates an instance of this command, passing the logger to write messages to.
        /// </summary>
        /// <param name="logger">The logger which messages should be written to.</param>
        public UpdateStatus(ILogger logger, ExternalComponent externalComponent, string thirdPartyStatus, bool componentAvailable)
            : base(logger)
        {
            ExternalComponent = externalComponent;
            ThirdPartyStatus = thirdPartyStatus;
            ComponentAvailable = componentAvailable;
        }

        protected override async Task<ExternalComponentStatus> OnExecute(IReadWriteDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();
            var updatedStatus = await databaseConnection.QuerySingleAsync<ExternalComponentStatus>("components.usp_UpdateStatus",
                new { External_Component_ID = ExternalComponent.Id, Status_Provider_ID = ExternalComponent.Status.StatusProvider, Raw_Status = ThirdPartyStatus, Available = ComponentAvailable },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return updatedStatus;
        }
    }
}

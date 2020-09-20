using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jibberwock.DataModels.ExternalComponents;
using Jibberwock.DataModels.ExternalComponents.Status;
using Jibberwock.Persistence.DataAccess.DataSources;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Jibberwock.Core.Background
{
    public class GatherComponentStatuses
    {
        private readonly SqlServerDataSource _dataSource;

        public GatherComponentStatuses(SqlServerDataSource dataSource) => _dataSource = dataSource;

        [FunctionName("GatherComponentStatuses")]
        public async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogDebug("Starting Atlassian Status processing");

            var getComponentsCommand = new Jibberwock.Persistence.DataAccess.Commands.ExternalComponents.GetByStatusProvider(log, StatusProvider.AtlassianStatus);

            var statusProviderComponents = await getComponentsCommand.Execute(_dataSource);

            // For Atlassian Status, the ExternalId property is in the form "{PageId}.{ComponentId}"
            // Our Atlassian Status package acts on the PageId basis - so for efficiency's sake, group them up.
            var statusPageIoPages = from component in statusProviderComponents
                                    let splitExternalId = component.ExternalId.Split('.')
                                    let pageId = splitExternalId[0]
                                    group component by pageId into groupedComponents
                                    select new
                                    {
                                        Components = groupedComponents.AsEnumerable(),
                                        StatusPageRetriever = new StatusPageClient.StatusPageClient(groupedComponents.Key)
                                    };
            
            // Note that we're running these in sequence. This is because we've only got a single SQL connection to work with here,
            // and can't run commands from multiple threads on the same SQL connection.
            foreach (var statusPage in statusPageIoPages)
                await processSingleStatusPage(log, statusPage.Components, statusPage.StatusPageRetriever);

            log.LogDebug("Atlassian Status processing completed");
        }

        private async Task processSingleStatusPage(ILogger log, IEnumerable<ExternalComponent> components, StatusPageClient.StatusPageClient statusPageRetriever)
        {
            log.LogDebug($"Processing page '{statusPageRetriever.PageId}'");

            try
            {
                await statusPageRetriever.RefreshAsync();

                // Get every matching component, matching on the case-insensitive component ID from ExternalComponent.ExternalId
                var matchingComponents = from comp in components
                                         let splitExternalId = comp.ExternalId.Split('.')
                                         let componentId = splitExternalId[1]
                                         join compStatus in statusPageRetriever.StatusPage.Components on componentId?.ToLowerInvariant() equals compStatus.Id.ToLowerInvariant()
                                         select new { ComponentRecord = comp, ExternalComponentDetails = compStatus };

                foreach (var component in matchingComponents)
                {
                    var componentAvailable = string.Equals(component.ExternalComponentDetails.Status.Indicator, "operational", StringComparison.InvariantCultureIgnoreCase);
                    var updateStatusCommand = new Jibberwock.Persistence.DataAccess.Commands.ExternalComponents.UpdateStatus(log, component.ComponentRecord, component.ExternalComponentDetails.Status.Indicator, componentAvailable);

                    log.LogInformation($"Atlassian Status component '{component.ExternalComponentDetails}' has status '{component.ExternalComponentDetails.Status}'. It is{(componentAvailable ? string.Empty : " not")} available");
                    log.LogDebug($"Recording the availability of Atlassian Status component '{component.ExternalComponentDetails}'");
                    await updateStatusCommand.Execute(_dataSource);
                    log.LogDebug($"Successfully recorded the availability of Atlassian Status component '{component.ExternalComponentDetails}'");
                }

                log.LogDebug($"Finished processing page '{statusPageRetriever.PageId}'");
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, $"Exception while processing page '{statusPageRetriever.PageId}'");
            }
        }
    }
}

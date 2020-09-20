using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Jibberwock.Core.Background
{
    public static class GatherComponentStatuses
    {
        [FunctionName("GatherComponentStatuses")]
        public static void Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogDebug("Starting Atlassian Status processing");

            log.LogDebug("Atlassian Status processing completed");
        }
    }
}

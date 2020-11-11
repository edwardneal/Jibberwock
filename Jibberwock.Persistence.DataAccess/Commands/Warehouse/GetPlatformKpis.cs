using Dapper;
using Jibberwock.DataModels.Warehouse;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Warehouse
{
    /// <summary>
    /// Gets platform-specific KPIs.
    /// </summary>
    public class GetPlatformKpis : ValidatingCommand<PlatformPerformanceIndicator, IReadableDataSource>
    {
        /// <summary>
        /// The period of time to get KPI data from.
        /// </summary>
        [Required]
        public TimeSpan AnalysisPeriod { get; set; }

        public GetPlatformKpis(ILogger logger, TimeSpan analysisPeriod)
            : base(logger)
        {
            AnalysisPeriod = analysisPeriod;
        }

        protected override async Task<PlatformPerformanceIndicator> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();
            var getKpiReader = await databaseConnection.QueryMultipleAsync("core.usp_GetPlatformKPIs",
                new { KPI_Period_Days = AnalysisPeriod.TotalDays },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            var kpis = await getKpiReader.ReadSingleAsync<PlatformPerformanceIndicator>();
            var dynAuditTrailRecords = await getKpiReader.ReadAsync();
            var auditTrailRecords = dynAuditTrailRecords.ToDictionary(d => (DateTime)d.Date, d => (long)d.EntryCount);

            kpis.ActivityByDate = auditTrailRecords;

            return kpis;
        }
    }
}

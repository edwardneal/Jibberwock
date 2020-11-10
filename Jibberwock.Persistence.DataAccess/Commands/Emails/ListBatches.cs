using Dapper;
using Jibberwock.DataModels.Core;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Emails
{
    /// <summary>
    /// Lists email batches.
    /// </summary>
    public class ListBatches : ValidatingCommand<IEnumerable<EmailBatch>, IReadableDataSource>
    {
        public ListBatches(ILogger logger)
            :base(logger)
        { }

        protected override async Task<IEnumerable<EmailBatch>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var results = await databaseConnection.QueryAsync<EmailBatch, EmailBatchType, EmailBatch>("core.usp_ListBatches",
                (eb, ebt) =>
                {
                    eb.Type = ebt;
                    return eb;
                }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return results;
        }
    }
}

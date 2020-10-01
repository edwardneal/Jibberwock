using Dapper;
using Jibberwock.DataModels.Products;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// Lists all product characteristics, without any kind of filter.
    /// </summary>
    public class ListAllCharacteristics : ValidatingCommand<IEnumerable<ProductCharacteristic>, IReadableDataSource>
    {
        /// <summary>
        /// Creates an instance of this command, passing the logger to write messages to.
        /// </summary>
        /// <param name="logger">The logger which messages should be written to.</param>
        public ListAllCharacteristics(ILogger logger)
            : base(logger)
        {
        }

        protected override async Task<IEnumerable<ProductCharacteristic>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();

            var results = await databaseConnection.QueryAsync<ProductCharacteristic>("products.usp_ListAllCharacteristics",
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return results;
        }
    }
}

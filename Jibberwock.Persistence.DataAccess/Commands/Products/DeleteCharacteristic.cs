using Dapper;
using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Security.Audit.EntryTypes;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.Commands.Auditing;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// Deletes a product characteristic.
    /// </summary>
    public class DeleteCharacteristic : AuditingCommand<DeleteCharacteristicStatusCode, Jibberwock.DataModels.Security.Audit.EntryTypes.DeleteProductCharacteristic>
    {
        /// <summary>
        /// The product characteristic to delete.
        /// </summary>
        [Required]
        public ProductCharacteristic ProductCharacteristic { get; set; }

        public DeleteCharacteristic(ILogger logger, User performedBy, string connectionId, long serviceId, string comment, ProductCharacteristic characteristic)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            ProductCharacteristic = characteristic;
        }

        protected override async Task<DeleteCharacteristicStatusCode> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, DeleteProductCharacteristic provisionalAuditTrailEntry)
        {
            if (ProductCharacteristic.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(ProductCharacteristic), "ProductCharacteristic.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var deletedCharacteristic = await databaseConnection.ExecuteScalarAsync<DeleteCharacteristicStatusCode>("products.usp_DeleteCharacteristic",
                new { Characteristic_ID = ProductCharacteristic.Id },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            provisionalAuditTrailEntry.ProductCharacteristic = ProductCharacteristic;

            return deletedCharacteristic;
        }
    }
}

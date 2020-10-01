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
    /// Creates a single product characteristic.
    /// </summary>
    public class CreateCharacteristic : AuditingCommand<ProductCharacteristic, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyProductCharacteristic>
    {
        /// <summary>
        /// The product characteristic details to create.
        /// </summary>
        [Required]
        public ProductCharacteristic ProductCharacteristic { get; set; }

        public CreateCharacteristic(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, ProductCharacteristic characteristic)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            ProductCharacteristic = characteristic;
        }

        protected override async Task<ProductCharacteristic> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, ModifyProductCharacteristic provisionalAuditTrailEntry)
        {
            if (ProductCharacteristic.Id != 0)
                throw new ArgumentOutOfRangeException(nameof(ProductCharacteristic), "ProductCharacteristic.Id must not have a value");
            if (string.IsNullOrWhiteSpace(ProductCharacteristic.Name))
                throw new ArgumentOutOfRangeException(nameof(ProductCharacteristic), "ProductCharacteristic.Name must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var resultantCharacteristic = await databaseConnection.QuerySingleAsync<ProductCharacteristic>("products.usp_CreateCharacteristic",
                new { Name = ProductCharacteristic.Name, 
                Description = ProductCharacteristic.Description, Visible = ProductCharacteristic.Visible,
                Enabled = ProductCharacteristic.Enabled },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            ProductCharacteristic = resultantCharacteristic;

            provisionalAuditTrailEntry.ProductCharacteristic = resultantCharacteristic;
            provisionalAuditTrailEntry.NewProductCharacteristic = true;

            return resultantCharacteristic;
        }
    }
}

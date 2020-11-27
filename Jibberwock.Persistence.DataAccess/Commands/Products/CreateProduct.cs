using Dapper;
using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Products.Configuration;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.Commands.Auditing;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// Creates a single product (with its associated applicable characteristics.)
    /// </summary>
    public class CreateProduct : AuditingCommand<Product, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyProduct>
    {
        /// <summary>
        /// The product details to create.
        /// </summary>
        [Required]
        public Product Product { get; set; }

        public CreateProduct(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, Product product)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Product = product;
        }

        protected override async Task<Product> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyProduct provisionalAuditTrailEntry)
        {
            if (Product.Id != 0)
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.Id must not have a value");
            if (string.IsNullOrWhiteSpace(Product.Name))
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.Name must have a value");
            if (string.IsNullOrWhiteSpace(Product.MoreInformationUrl))
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.MoreInformationUrl must have a value");
            if (string.IsNullOrWhiteSpace(Product.ConfigurationControlName))
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.ConfigurationControlName must have a value");
            if (Product.DefaultProductConfiguration == null)
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.DefaultProductConfiguration must have a value");
            if (Product.DefaultProductConfiguration.Id != 0)
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.DefaultProductConfiguration.Id must not have a value");
            if (string.IsNullOrWhiteSpace(Product.DefaultProductConfiguration.ConfigurationString))
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.DefaultProductConfiguration.ConfigurationString must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var characteristicParameter = (from c in Product.ApplicableCharacteristics
                                           select new Jibberwock.Persistence.DataAccess.TableTypes.Products.ProductCharacteristic(c.Id))
                                           .AsTableValuedParameter("products.udt_ProductCharacteristic");
            using var productCreationBatch = await databaseConnection.QueryMultipleAsync("products.usp_CreateProduct",
                new
                {
                    Service_ID = OriginatingService.Id,
                    Name = Product.Name,
                    Description = Product.Description,
                    More_Information_URL = Product.MoreInformationUrl,
                    Visible = Product.Visible,
                    Configuration_Control_Name = Product.ConfigurationControlName,
                    Configuration_String = Product.DefaultProductConfiguration.ConfigurationString,
                    Characteristics = characteristicParameter
                },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            var resultantProduct = await productCreationBatch.ReadSingleOrDefaultAsync<Product>();
            var resultantProductConfig = await productCreationBatch.ReadSingleOrDefaultAsync<RawProductConfiguration>();
            var resultantCharacteristics = await productCreationBatch.ReadAsync<ProductCharacteristic>();

            resultantProduct.DefaultProductConfiguration = resultantProductConfig;
            resultantProduct.ApplicableCharacteristics = resultantCharacteristics;

            Product = resultantProduct;

            provisionalAuditTrailEntry.Product = resultantProduct;
            provisionalAuditTrailEntry.NewProduct = true;

            return resultantProduct;
        }
    }
}

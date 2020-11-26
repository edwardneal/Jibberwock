using Dapper;
using Jibberwock.DataModels.Products;
using Jibberwock.DataModels.Products.Configuration;
using Jibberwock.DataModels.Users;
using Jibberwock.Persistence.DataAccess.DataSources;
using Jibberwock.Persistence.DataAccess.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// Gets a single product by its ID, including the details of its applicable characteristics.
    /// </summary>
    public class GetById : ValidatingCommand<Product, IReadableDataSource>
    {
        /// <summary>
        /// The product to get the details of
        /// </summary>
        [Required]
        public Product Product { get; set; }

        /// <summary>
        /// The user which will be used for permission/security checks.
        /// </summary>
        [Required]
        public User CurrentUser { get; set; }

        public GetById(ILogger logger, User currentUser, Product product)
            : base(logger)
        {
            CurrentUser = currentUser;
            Product = product;
        }

        protected override async Task<Product> OnExecute(IReadableDataSource dataSource)
        {
            if (CurrentUser.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(CurrentUser), "User.Id must have a value");
            if (Product.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Product), "Product.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var productRetrieval = await databaseConnection.QueryMultipleAsync("products.usp_GetProductById",
                new
                {
                    Product_ID = Product.Id,
                    User_ID = CurrentUser.Id
                },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            var resultantProduct = await productRetrieval.ReadSingleOrDefaultAsync<Product>();
            var resultantProductConfig = await productRetrieval.ReadSingleOrDefaultAsync<RawProductConfiguration>();
            var resultantCharacteristics = await productRetrieval.ReadAsync<ProductCharacteristic>();

            resultantProduct.ApplicableCharacteristics = resultantCharacteristics;
            resultantProduct.DefaultProductConfiguration = resultantProductConfig;

            Product = resultantProduct;

            return resultantProduct;
        }
    }
}

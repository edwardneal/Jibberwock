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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// Lists all products which are accessible to the general public.
    /// </summary>
    public class ListPublicProducts : ValidatingCommand<IEnumerable<Product>, IReadableDataSource>
    {

        public ListPublicProducts(ILogger logger)
            : base(logger)
        { }

        protected override async Task<IEnumerable<Product>> OnExecute(IReadableDataSource dataSource)
        {
            var databaseConnection = await dataSource.GetDbConnection();
            var resultSet = await databaseConnection.QueryMultipleAsync("products.usp_ListPublicProducts",
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            // The first result set is pretty simple - we're just performing a straightforward mapping from a result set to 
            // the list of products.
            var products = resultSet.Read<Product, RawProductConfiguration, Product>((p, rpc) =>
            {
                p.DefaultProductConfiguration = rpc;
                return p;
            });

            // The second result set is a bit trickier, since we've got to join them together, then
            // convert from a dynamic object into product tiers
            IEnumerable<dynamic> productTiers = await resultSet.ReadAsync();
            var groupedTiers = from prod in products
                                         join pT in productTiers on prod.Id equals pT.ProductId into gj
                                         select new
                                         {
                                             Product = prod,
                                             Tiers = (from pT in gj
                                                                select new Tier()
                                                                {
                                                                    Id = pT.Id,
                                                                    ExternalId = pT.ExternalId,
                                                                    Name = pT.Name,
                                                                    Visible = pT.Visible,
                                                                    StartDate = pT.StartDate,
                                                                    EndDate = pT.EndDate
                                                                }).ToArray()
                                         };

            foreach (var prod in groupedTiers)
            {
                prod.Product.Tiers = prod.Tiers;
            }

            return products;
        }
    }
}

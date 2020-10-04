using Dapper;
using Jibberwock.DataModels.Products;
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
    /// List all tiers in a product, excluding hidden tiers by default.
    /// </summary>
    public class ListTiers : ValidatingCommand<IEnumerable<Tier>, IReadableDataSource>
    {
        /// <summary>
        /// The product to list the tiers of.
        /// </summary>
        [Required]
        public Product Product { get; set; }

        /// <summary>
        /// If true, executing this command will include hidden product tiers.
        /// </summary>
        [Required]
        public bool IncludeHiddenTiers { get; set; }

        /// <summary>
        /// The user which will be used for permission/security checks.
        /// </summary>
        [Required]
        public User CurrentUser { get; set; }

        public ListTiers(ILogger logger, User currentUser, bool includeHiddenTiers, Product product)
            : base(logger)
        {
            CurrentUser = currentUser;
            IncludeHiddenTiers = includeHiddenTiers;
            Product = product;
        }

        protected override async Task<IEnumerable<Tier>> OnExecute(IReadableDataSource dataSource)
        {
            if (CurrentUser.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(User), "User.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var listTiersBatch = await databaseConnection.QueryMultipleAsync("products.usp_ListProductTiers",
                new { Product_ID = Product.Id, Include_Hidden = IncludeHiddenTiers, User_ID = CurrentUser.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            // The first result set is simple enough: a mapped set of Tiers
            var productTiers = await listTiersBatch.ReadAsync<Tier>();

            // The second result set is a bit more complex: convert from a dynamic object into a
            // tier product characteristic value
            IEnumerable<dynamic> tierCharacteristicValues = await listTiersBatch.ReadAsync();
            var groupedTCVs = from tier in productTiers
                              join tcv in tierCharacteristicValues on tier.Id equals tcv.TierId into gj
                              select new
                              {
                                  Tier = tier,
                                  TierCharacteristicValues = (from tcv in gj
                                                              select new TierProductCharacteristic()
                                                              {
                                                                  Id = tcv.TierCharacteristicValueId,
                                                                  CharacteristicValue = tcv.Value,
                                                                  ProductCharacteristic = new ProductCharacteristic()
                                                                  {
                                                                      Id = tcv.CharacteristicId,
                                                                      Name = tcv.Name,
                                                                      Description = tcv.Description,
                                                                      Visible = tcv.Visible,
                                                                      Enabled = tcv.Enabled,
                                                                      ValueType = (ProductCharacteristicValueType) tcv.ValueType
                                                                  }
                                                              }).ToArray()
                              };

            foreach (var tier in groupedTCVs)
                tier.Tier.Characteristics = tier.TierCharacteristicValues;
            
            return productTiers;
        }
    }
}

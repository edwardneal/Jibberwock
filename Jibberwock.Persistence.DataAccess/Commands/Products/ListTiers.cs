using Dapper;
using Jibberwock.DataModels.Products;
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

        public ListTiers(ILogger logger, User currentUser, bool includeHiddenTiers)
            : base(logger)
        {
            CurrentUser = currentUser;
            IncludeHiddenTiers = includeHiddenTiers;
        }

        protected override async Task<IEnumerable<Tier>> OnExecute(IReadableDataSource dataSource)
        {
            if (CurrentUser.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(User), "User.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var productTiers = await databaseConnection.QueryAsync<Tier>("products.usp_ListProductTiers",
                new { Product_ID = Product.Id, Include_Hidden = IncludeHiddenTiers, User_ID = CurrentUser.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            
            return productTiers;
        }
    }
}

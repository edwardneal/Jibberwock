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
    /// Lists products, excluding hidden products by default.
    /// </summary>
    public class ListProducts : ValidatingCommand<IEnumerable<Product>, IReadableDataSource>
    {
        /// <summary>
        /// If true, executing this command will include hidden products.
        /// </summary>
        [Required]
        public bool IncludeHiddenProducts { get; set; }

        /// <summary>
        /// The user which will be used for permission/security checks.
        /// </summary>
        [Required]
        public User CurrentUser { get; set; }

        public ListProducts(ILogger logger, User currentUser, bool includeHiddenProducts)
            : base(logger)
        {
            CurrentUser = currentUser;
            IncludeHiddenProducts = includeHiddenProducts;
        }

        protected override async Task<IEnumerable<Product>> OnExecute(IReadableDataSource dataSource)
        {
            if (CurrentUser.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(User), "User.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var results = await databaseConnection.QueryAsync<Product>("products.usp_ListProducts",
                new { Include_Hidden = IncludeHiddenProducts, User_ID = CurrentUser.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            return results;
        }
    }
}

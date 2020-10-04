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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    /// <summary>
    /// Gets a specific tier, including the values for any product characteristics.
    /// </summary>
    public class GetTier : ValidatingCommand<Tier, IReadableDataSource>
    {
        /// <summary>
        /// The tiers to get the details of.
        /// </summary>
        [Required]
        public Tier Tier { get; set; }

        /// <summary>
        /// The user which will be used for permission/security checks.
        /// </summary>
        [Required]
        public User CurrentUser { get; set; }

        public GetTier(ILogger logger, User currentUser, Tier tier)
            : base(logger)
        {
            CurrentUser = currentUser;
            Tier = tier;
        }

        protected override async Task<Tier> OnExecute(IReadableDataSource dataSource)
        {
            if (CurrentUser.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(CurrentUser), "User.Id must have a value");
            if (Tier.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tier), "Tier.Id must have a value");
            if (Tier.Product == null)
                throw new ArgumentOutOfRangeException(nameof(Tier), "Tier.Product must have a value");
            if (Tier.Product.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tier), "Tier.Product.Id must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var productTierDetails = await databaseConnection.QueryMultipleAsync("products.usp_GetProductTierById",
                new { Product_ID = Tier.Product.Id, Tier_ID = Tier.Id, User_ID = CurrentUser.Id },
                commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);

            var resultantTier = await productTierDetails.ReadSingleAsync<Tier>();
            var characteristicValues = await productTierDetails.ReadAsync<dynamic>();

            resultantTier.Characteristics = (from cV in characteristicValues
                                             select new TierProductCharacteristic()
                                             {
                                                 Id = cV.Id,
                                                 CharacteristicValue = cV.CharacteristicValue,
                                                 ProductCharacteristic = new ProductCharacteristic()
                                                 {
                                                     Id = cV.CharacteristicId,
                                                     Name = cV.Name,
                                                     Description = cV.Description,
                                                     Visible = cV.Visible,
                                                     Enabled = cV.Enabled,
                                                     ValueType = (ProductCharacteristicValueType)cV.ValueType
                                                 }
                                             }).ToArray();

            Tier = resultantTier;

            return resultantTier;
        }
    }
}

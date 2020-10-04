using Dapper;
using Jibberwock.DataModels.Products;
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
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.Commands.Products
{
    public class UpdateTier : AuditingCommand<Tier, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyTier>
    {
        /// <summary>
        /// The product tier details to create.
        /// </summary>
        [Required]
        public Tier Tier { get; set; }

        public UpdateTier(ILogger logger, User performedBy, string connectionId, int serviceId, string comment, Tier tier)
            : base(logger, performedBy, connectionId, serviceId, comment)
        {
            Tier = tier;
        }

        protected override async Task<Tier> OnAuditedExecute(IReadWriteDataSource dataSource, IDbTransaction transaction, Jibberwock.DataModels.Security.Audit.EntryTypes.ModifyTier provisionalAuditTrailEntry)
        {
            if (Tier.Id == 0)
                throw new ArgumentOutOfRangeException(nameof(Tier), "Tier.Id must have a value");
            if (string.IsNullOrWhiteSpace(Tier.Name))
                throw new ArgumentOutOfRangeException(nameof(Tier), "Tier.Name must have a value");

            var databaseConnection = await dataSource.GetDbConnection();

            var characteristicParameter = (from c in Tier.Characteristics
                                           select new Jibberwock.Persistence.DataAccess.TableTypes.Products.TierCharacteristicValue(c.ProductCharacteristic.Id, c.CharacteristicValue))
                                           .AsTableValuedParameter("products.udt_TierCharacteristicValue");
            var updateTierResultSet = await databaseConnection.QueryMultipleAsync("products.usp_UpdateProductTier",
                new
                {
                    Product_ID = Tier.Product.Id,
                    Tier_ID = Tier.Id,
                    User_ID = PerformedBy.Id,
                    Name = Tier.Name,
                    Visible = Tier.Visible,
                    Start_Date = Tier.StartDate,
                    End_Date = Tier.EndDate,
                    Characteristic_Values = characteristicParameter
                },
                transaction: transaction, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 30);
            var resultantTier = await updateTierResultSet.ReadSingleAsync<Tier>();
            var characteristicValues = await updateTierResultSet.ReadAsync<dynamic>();

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

            provisionalAuditTrailEntry.Tier = resultantTier;
            provisionalAuditTrailEntry.NewTier = false;

            return Tier;
        }
    }
}

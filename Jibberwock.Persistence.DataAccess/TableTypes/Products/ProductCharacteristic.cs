using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.TableTypes.Products
{
    internal class ProductCharacteristic : UserDefinedTableType
    {
        public long CharacteristicId { get; set; }

        public ProductCharacteristic(long characteristicId)
            : base(GetColumnMetadata<long>("Characteristic_ID"))
        {
            base.SetValue<long>(0, characteristicId);

            CharacteristicId = characteristicId;
        }
    }
}

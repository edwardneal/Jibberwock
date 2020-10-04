using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.TableTypes.Products
{
    internal class TierCharacteristicValue : UserDefinedTableType
    {
        public int CharacteristicId { get; set; }

        public object CharacteristicValue { get; set; }

        public TierCharacteristicValue(int characteristicId, object characteristicValue)
            : base(GetColumnMetadata<int>("Characteristic_ID"), GetColumnMetadata<object>("Characteristic_Value"))
        {
            base.SetValue<int>(0, characteristicId);
            base.SetValue<object>(1, characteristicValue);

            CharacteristicId = characteristicId;
            CharacteristicValue = characteristicValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.ActionModels.Products
{
    /// <summary>
    /// Describes one of the characteristic values of <see cref="TierCreationOptions"/>.
    /// </summary>
    public class TierCharacteristicValue
    {
        /// <summary>
        /// The ID of the characteristic to set a value for.
        /// </summary>
        /// <remarks>
        /// This characteristic specifies a datatype, and the <see cref="Value"/> property must comply with this datatype.
        /// </remarks>
        public int CharacteristicId { get; set; }

        /// <summary>
        /// The value of this characteristic for this tier.
        /// </summary>
        public string Value { get; set; }
    }
}

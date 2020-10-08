using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// Represents a specific type of <see cref="EmailBatch"/> and the email settings which are associated with this type.
    /// </summary>
    public class EmailBatchType
    {
        /// <summary>
        /// The unique internal identifier for this <see cref="EmailBatchType"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of this <see cref="EmailBatchType"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The From: address of all emails in the associated <see cref="EmailBatch"/>.
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// The From: label of all emails in the associated <see cref="EmailBatch"/>.
        /// </summary>
        public string SenderContact { get; set; }

        /// <summary>
        /// Provider-specific "unsubscription group" which enables people to selectively unsubscribe from receiving emails of this <see cref="EmailBatchType"/>.
        /// </summary>
        public int UnsubscriptionGroupId { get; set; }

        /// <summary>
        /// Provider-specific template for all emails of this <see cref="EmailBatchType"/>.
        /// </summary>
        public string MessageTemplateId { get; set; }
    }
}

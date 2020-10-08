using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// Represents a group of emails of a specific type which are to be sent.
    /// </summary>
    public class EmailBatch
    {
        /// <summary>
        /// The unique internal identifier for this <see cref="EmailBatch"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// This <see cref="EmailBatch"/>'s type (and associated metadata.)
        /// </summary>
        public EmailBatchType Type { get; set; }

        /// <summary>
        /// The ID of the message sent via Service Bus to the background task to process this <see cref="EmailBatch"/>.
        /// </summary>
        public string ServiceBusMessageId { get; set; }

        /// <summary>
        /// The first time that the background task attempted to process this <see cref="EmailBatch"/>.
        /// </summary>
        public DateTimeOffset? DateFirstProcessed { get; set; }

        /// <summary>
        /// The most recent time that the background task has attempted to process this <see cref="EmailBatch"/>.
        /// </summary>
        public DateTimeOffset? DateLastProcessed { get; set; }

        /// <summary>
        /// Has this <see cref="EmailBatch"/> been processed successfully (<c>true</c>), has it failed (<c>false</c>)
        /// or has it not been processed yet (<c>null</c>)
        /// </summary>
        public bool? ProcessedSuccessfully { get; set; }
    }
}

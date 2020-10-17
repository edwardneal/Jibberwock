using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Core
{
    /// <summary>
    /// Represents a specific email sent as a result of processing an <see cref="EmailBatch"/>.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// The unique internal identifier for this <see cref="Email"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The <see cref="EmailBatch"/> which resulted in this <see cref="Email"/> being sent.
        /// </summary>
        public EmailBatch SourceBatch { get; set; }

        /// <summary>
        /// The date that this <see cref="Email"/> was sent.
        /// </summary>
        public DateTimeOffset? SendDate { get; set; }

        /// <summary>
        /// The unique identifier for this <see cref="Email"/> in the system which sent it.
        /// </summary>
        public string ExternalEmailId { get; set; }
    }
}

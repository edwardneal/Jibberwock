using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert
{
    /// <summary>
    /// Represents a single alert raised through the execution of an <see cref="AlertDefinition"/>.
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// The unique internal reference of this <see cref="Alert"/>.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The friendly description of this <see cref="Alert"/>.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The <see cref="AlertDefinition"/> which generated this <see cref="Alert"/>.
        /// </summary>
        public AlertDefinition AlertDefinition { get; set; }

        /// <summary>
        /// The system-generated date/time when this <see cref="Alert"/> was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// The user-provided date/time when this <see cref="Alert"/> occurred.
        /// </summary>
        public DateTimeOffset DateRaised { get; set; }

        /// <summary>
        /// This <see cref="Alert"/>'s priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Metadata associated with this <see cref="Alert"/>.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// An optional unique identifier to distinguish this <see cref="Alert"/> from other instances of the same <see cref="AlertDefinition"/>.
        /// </summary>
        /// <remarks>
        /// This will generally be used when processing "bulk" data for multiple objects, where many instances of the same object could have an <see cref="Alert"/>.
        /// </remarks>
        public string InstanceId { get; set; }
    }
}

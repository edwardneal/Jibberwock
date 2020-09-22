using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert
{
    /// <summary>
    /// This references a compiled alert recognition snippet from an <see cref="AlertDefinition"/>.
    /// </summary>
    public class CompiledRecognitionSnippet
    {
        /// <summary>
        /// The unique identifier of this <see cref="CompiledRecognitionSnippet"/>.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// This <see cref="CompiledRecognitionSnippet"/>'s parent <see cref="AlertDefinition"/>.
        /// </summary>
        public AlertDefinition AlertDefinition { get; set; }

        /// <summary>
        /// The download URL of this compiled recognition snippet.
        /// </summary>
        public string CompiledSnippetUrl { get; set; }
    }
}

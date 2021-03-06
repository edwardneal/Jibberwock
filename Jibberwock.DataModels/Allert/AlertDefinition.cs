﻿using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert
{
    /// <summary>
    /// Represents a snippet of .NET code which runs against an input and returns a number of <see cref="Alert"/>s.
    /// </summary>
    public class AlertDefinition : SecurableResource
    {
        /// <summary>
        /// The friendly name of this <see cref="AlertDefinition"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Default priority of <see cref="Alert"/>s generated by this <see cref="AlertDefinition"/>.
        /// </summary>
        public int DefaultPriority { get; set; }

        /// <summary>
        /// A brief summary of this <see cref="AlertDefinition"/>'s functionality.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Any extra metadata to be associated with <see cref="Alert"/>s generated by this <see cref="AlertDefinition"/>.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The script to be run against an input. Compiled into <see cref="CompiledRecognitionSnippet"/>.
        /// </summary>
        public string RecognitionSnippet { get; set; }

        /// <summary>
        /// The expected language of <see cref="RecognitionSnippet"/>.
        /// </summary>
        public AlertRecognitionSnippetLanguage RecognitionSnippetLanguage { get; set; }

        /// <summary>
        /// The result of compiling <see cref="RecognitionSnippet"/>.
        /// </summary>
        public CompiledRecognitionSnippet CompiledRecognitionSnippet { get; set; }
    }
}

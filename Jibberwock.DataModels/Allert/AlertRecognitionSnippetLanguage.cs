using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Allert
{
    /// <summary>
    /// Describes the possible languages which <see cref="AlertDefinition.RecognitionSnippet"/> could be written in.
    /// </summary>
    public enum AlertRecognitionSnippetLanguage
    {
        /// <summary>
        /// C#
        /// </summary>
        CSharp = 1,
        /// <summary>
        /// Visual Basic.NET
        /// </summary>
        VBDotNet = 2,
        /// <summary>
        /// F#
        /// </summary>
        FSharp = 3
    }
}

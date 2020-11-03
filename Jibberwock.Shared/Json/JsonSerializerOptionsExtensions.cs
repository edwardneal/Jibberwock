using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.Shared.Json
{
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Makes these <see cref="JsonSerializerOptions"/> the default set of options for all use of <see cref="JsonSerializer"/>.
        /// </summary>
        /// <param name="options"></param>
        public static void MakeDefault(this JsonSerializerOptions options)
        {
            var fieldInfo = typeof(JsonSerializerOptions).GetField("s_defaultOptions",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            if (fieldInfo == null)
            {
                throw new InvalidOperationException("Could not find an s_defaultOptions hidden field on JsonSerializerOptions");
            }

            fieldInfo.SetValue(null, options);
        }
    }
}

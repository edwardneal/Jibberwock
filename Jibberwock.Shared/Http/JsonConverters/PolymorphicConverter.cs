using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jibberwock.Shared.Http.JsonConverters
{
    /// <summary>
    /// Enables the serialisation of derived classes as JSON.
    /// </summary>
    public class PolymorphicConverter<T> : JsonConverter<T>
    {
        /// <inheritdoc />
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (T)JsonSerializer.Deserialize(ref reader, typeToConvert, options);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

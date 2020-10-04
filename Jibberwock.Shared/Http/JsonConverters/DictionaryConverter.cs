using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jibberwock.Shared.Http.JsonConverters
{
    /// <summary>
    /// Enables the serialisation and deserialisation of dictionaries as JSON.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class DictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>>
    {
        public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            { throw new JsonException($"Token type for {this.GetType().Name} was not StartArray (actual type was {reader.TokenType})"); }

            // Step forward, into the property values
            reader.Read();

            var dictionary = new Dictionary<TKey, TValue>();

            // Keep stepping through the reader until we hit the EndObject
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                TKey key = default;
                TValue value = default;

                if (reader.TokenType != JsonTokenType.PropertyName)
                { throw new JsonException($"Token type for {this.GetType().Name} was not PropertyName (actual type was {reader.TokenType})"); }

                key = JsonSerializer.Deserialize<TKey>(reader.ValueSpan, options);

                // Step forwards from the property name to the value
                reader.Read();

                value = JsonSerializer.Deserialize<TValue>(ref reader, options);

                // Step forwards to the next property name
                reader.Read();

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
        {
            // Write the start of a JSON object
            writer.WriteStartObject();

            // Iterate through every key-value pair, writing the key as a property name and the value as the property's value
            foreach(var key in value.Keys)
            {
                writer.WritePropertyName(JsonSerializer.Serialize(key, options));
                JsonSerializer.Serialize(writer, value[key], options);
            }

            // Close the object we've started
            writer.WriteEndObject();
        }
    }
}

using Jibberwock.DataModels.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Jibberwock.Shared.Http.JsonConverters
{
    /// <summary>
    /// Enables the serialisation and deserialisation of <see cref="SecurableResource"/> instances.
    /// </summary>
    public class SecurableResourceConverter : PolymorphicConverter<SecurableResource>
    {
        /// <inheritdoc />
        public override SecurableResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            SecurableResource readResource = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            { throw new JsonException($"Token type for {this.GetType().Name} was not StartObject (actual type was {reader.TokenType})"); }

            // Step forward, into the property values
            reader.Read();

            var rawValues = new Dictionary<string, byte[]>();

            // Keep stepping forward through the reader until we hit the EndObject
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                { throw new JsonException($"Token type for {this.GetType().Name} was not PropertyName (actual type was {reader.TokenType})"); }

                // Get the property names as strings. We'll keep track of these later
                var propertyName = Encoding.UTF8.GetString(reader.ValueSpan);

                // Step forward from the property name into its value
                reader.Read();

                var propertyValue = reader.ValueSpan;

                if (reader.TokenType != JsonTokenType.Null)
                { rawValues.Add(propertyName, propertyValue.ToArray()); }

                // Step forward again to the next property name
                reader.Read();
            }

            // Retrieve and convert the resource type
            var resourceTypeKey = options.PropertyNamingPolicy.ConvertName(nameof(SecurableResource.ResourceType));

            if (! rawValues.TryGetValue(resourceTypeKey, out var resourceTypeBytes))
            { throw new JsonException($"Could not find a property named \"{resourceTypeKey}\"."); }

            var resourceType = JsonSerializer.Deserialize<SecurableResourceType>(resourceTypeBytes.AsSpan(), options);

            readResource = createSecurableResource(resourceType, rawValues, options);

            return readResource;
        }

        private SecurableResource createSecurableResource(SecurableResourceType type, Dictionary<string, byte[]> rawPropertyValues, JsonSerializerOptions options)
        {
            SecurableResource readResource = type switch
            {
                SecurableResourceType.Tenant => new Jibberwock.DataModels.Tenants.Tenant(),
                SecurableResourceType.ApiKey => new Jibberwock.DataModels.Tenants.ApiKey(),
                SecurableResourceType.Product => new Jibberwock.DataModels.Products.Product(),
                SecurableResourceType.Service => new Jibberwock.DataModels.Core.Service(),
                SecurableResourceType.Allert_AlertDefinition => new Jibberwock.DataModels.Allert.AlertDefinition(),
                SecurableResourceType.Allert_AlertDefinitionGroup => new Jibberwock.DataModels.Allert.AlertDefinitionGroup(),
                _ => throw new JsonException($"Unable to create a SecurableResource from type {type}."),
            };

            var clrType = readResource.GetType();
            var properties = clrType.GetProperties();

            foreach (var pi in properties)
            {
                var expectedPropertyName = options.PropertyNamingPolicy.ConvertName(pi.Name);
                byte[] rawPropertyValue;

                // If we're using a case-insensitive property value, we have to iterate the dictionary manually.
                // Otherwise, we can take a shortcut via a key lookup.
                if (options.PropertyNameCaseInsensitive)
                {
                    var kvp = rawPropertyValues.FirstOrDefault(kvp => string.Equals(kvp.Key, expectedPropertyName, StringComparison.OrdinalIgnoreCase));

                    if (kvp.Value == null)
                    { continue; }

                    rawPropertyValue = kvp.Value;
                }
                else
                {
                    if (! rawPropertyValues.TryGetValue(expectedPropertyName, out rawPropertyValue))
                    { continue; }
                }

                object deserialisedValue;
                if (pi.PropertyType == typeof(string) && rawPropertyValue.Length > 0)
                { deserialisedValue = Encoding.UTF8.GetString(rawPropertyValue); }
                else
                { deserialisedValue = JsonSerializer.Deserialize(rawPropertyValue.AsSpan(), pi.PropertyType, options); }

                pi.SetValue(readResource, deserialisedValue);
            }

            return readResource;
        }
    }
}

using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OriNoco.Serializer
{
    public class Vector2Serializer : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"Expected StartObject, but found {reader.TokenType} at {reader.TokenStartIndex}.");

            float x = 0;
            float y = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new Vector2(x, y);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString()!;

                    reader.Read();
                    switch (propertyName)
                    {
                        case "x":
                            x = reader.GetSingle();
                            break;
                        case "y":
                            y = reader.GetSingle();
                            break;
                        default:
                            throw new JsonException($"Unexpected property: {propertyName}");
                    }
                }
            }

            throw new JsonException("End of object not found.");
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteNumberValue(value.X);
            writer.WritePropertyName("y");
            writer.WriteNumberValue(value.Y);
            writer.WriteEndObject();
        }
    }
}
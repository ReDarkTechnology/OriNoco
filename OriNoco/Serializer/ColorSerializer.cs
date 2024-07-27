using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OriNoco.Serializer
{
    public class ColorFSerializer : JsonConverter<ColorF>
    {
        public override ColorF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"Expected StartObject, but found {reader.TokenType} at {reader.TokenStartIndex}.");

            float r = 0;
            float g = 0;
            float b = 0;
            float a = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new ColorF(r, g, b, a);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString()!;

                    reader.Read();
                    switch (propertyName)
                    {
                        case "r":
                            r = reader.GetSingle();
                            break;
                        case "g":
                            g = reader.GetSingle();
                            break;
                        case "b":
                            b = reader.GetSingle();
                            break;
                        case "a":
                            a = reader.GetSingle();
                            break;
                        default:
                            if (options.UnmappedMemberHandling == JsonUnmappedMemberHandling.Disallow)
                                throw new JsonException($"Unexpected property: {propertyName}");
                            break;
                    }
                }
            }

            throw new JsonException("End of object not found.");
        }

        public override void Write(Utf8JsonWriter writer, ColorF value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteNumberValue(value.R);
            writer.WritePropertyName("g");
            writer.WriteNumberValue(value.G);
            writer.WritePropertyName("b");
            writer.WriteNumberValue(value.B);
            writer.WritePropertyName("a");
            writer.WriteNumberValue(value.A);
            writer.WriteEndObject();
        }
    }
}
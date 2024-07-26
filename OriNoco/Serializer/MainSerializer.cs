using System.Text.Json;
using System.Text.Json.Serialization;

namespace OriNoco.Serializer
{
    public static class MainSerializer
    {
        public static string Serialize(object obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions() { 
                WriteIndented = indented,
                Converters = {
                    new Vector2Serializer(),
                    new ColorFSerializer()
                }
            });
        }

        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions() { 
                Converters = {
                    new Vector2Serializer(),
                    new ColorFSerializer()
                },
                WriteIndented = true,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            });
        }
    }
}

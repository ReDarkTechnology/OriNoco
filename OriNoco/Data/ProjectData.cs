using System;
using System.Text.Json.Serialization;

namespace OriNoco.Data
{
    [Serializable]
    public class ProjectData
    {
        [JsonPropertyName("gridScale")]
        public float GridScale { get; set; } = 240f;

        [JsonPropertyName("division")]
        public int Division { get; set; } = 4;

        [JsonPropertyName("gridCount")]
        public int GridCount { get; set; } = 64;
    }
}

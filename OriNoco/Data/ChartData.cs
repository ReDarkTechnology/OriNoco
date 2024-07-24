using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OriNoco.Data
{
    [Serializable]
    public class ChartData
    {
        [JsonPropertyName("info")]
        public ChartInfoData Info { get; set; } = new();
        [JsonPropertyName("speed")]
        public float Speed { get; set; } = 1f;
        [JsonPropertyName("bpm")]
        public float BPM { get; set; } = 120f;
        [JsonPropertyName("speeds")]
        public List<SpeedData> Speeds { get; set; } = [];
        [JsonPropertyName("bpms")]
        public List<BPMData> BPMs { get; set; } = [];
        [JsonPropertyName("notes")]
        public List<NoteData> Notes { get; set; } = [];
    }
}

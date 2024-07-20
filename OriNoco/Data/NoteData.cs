using OriNoco.Rhine;
using System;
using System.Numerics;
using System.Text.Json.Serialization;

namespace OriNoco.Data
{
    [Serializable]
    public class NoteData
    {
        [JsonPropertyName("position")]
        public Vector2 Position { get; set; }
        [JsonPropertyName("time")]
        public float Time { get; set; }
        [JsonPropertyName("direction")]
        public Direction Direction { get; set; }
        [JsonPropertyName("type")]
        public NoteType Type { get; set; }

        public NoteData() { }
        public NoteData(RhineNote note)
        {
            Position = note.note.Position;
            Time = note.time;
            Direction = note.direction;
            Type = note.type;
        }
    }
}

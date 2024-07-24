using System;
using System.Numerics;
using System.Text.Json.Serialization;
using OriNoco.Rhine;

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
        public Direction Direction { get; set; } = Direction.Up;
        [JsonPropertyName("type")]
        public NoteType Type { get; set; } = NoteType.Tap;
        [JsonPropertyName("persistPosition")]
        public bool PersistPosition { get; set; } = false;

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

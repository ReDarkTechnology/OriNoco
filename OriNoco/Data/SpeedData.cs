using System;
using System.Text.Json.Serialization;

namespace OriNoco.Data
{
    [Serializable]
    public class SpeedData
    {
        [JsonPropertyName("time")]
        public float Time { get; set; }
        [JsonPropertyName("speed")]
        public float Speed { get; set; } = 1f;

        public SpeedData() { }
        public SpeedData(float time, float speed)
        {
            Time = time;
            Speed = speed;
        }

        public SpeedData(LaneChange change)
        {
            Time = change.time;
            Speed = change.rate;
        }

        public LaneChange ToChange() => new(Time, Speed);
    }
}

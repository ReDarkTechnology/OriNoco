using System;
using System.Text.Json.Serialization;

namespace OriNoco.Data
{
    [Serializable]
    public class BPMData
    {
        [JsonPropertyName("time")]
        public float Time { get; set; }
        [JsonPropertyName("bpm")]
        public float BPM { get; set; } = 120f;

        public BPMData() { }
        public BPMData(float time, float bpm)
        {
            Time = time;
            BPM = bpm;
        }

        public BPMData(LaneChange change)
        {
            Time = change.time;
            BPM = Core.RateToBPM(change.rate);
        }

        public BPMData(BPMChange change)
        {
            Time = change.time;
            BPM = change.bpm;
        }

        public LaneChange ToChange() => new(Time, Core.BPMToRate(BPM));
        public BPMChange ToBPMChange() => new(Time, BPM);
    }
}

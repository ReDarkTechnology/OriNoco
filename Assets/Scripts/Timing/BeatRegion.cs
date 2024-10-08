using System;

namespace OriNoco.Timing
{
    /// <summary>
    /// Represents a region in the chart metronome, defined by a start time, tempo (beats per minute), and default time
    /// signature.
    /// </summary>
    [Serializable]
    public class BeatRegion
    {
        public BeatTime time = new(0, 1, 1);
        public float beatPerMinute = 120f;
        public float defaultSignature = 4;

        public BeatRegion() { }
        public BeatRegion(BeatTime time, float bpm = 120, float signature = 4) =>
            (this.time, beatPerMinute, defaultSignature) = (time, bpm, signature);

        public override string ToString()
        {
            return $"{time}-{beatPerMinute}:{defaultSignature}";
        }
    }
}
namespace OriNoco.Timing
{
    public class BeatRegion
    {
        public BeatTime time = new(0, 1, 1);
        public float beatPerMinute = 120f;
        public float defaultSignature = 4;

        public BeatRegion() { }
        public BeatRegion(BeatTime time, float bpm = 120, float signature = 4)
        {
            this.time = time;
            this.beatPerMinute = bpm;
            this.defaultSignature = signature;
        }

        public override string ToString()
        {
            return $"{time}-{beatPerMinute}:{defaultSignature}";
        }
    }
}
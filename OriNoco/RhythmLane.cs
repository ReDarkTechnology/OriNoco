namespace OriNoco
{
    public class RhythmLane : PredictableLane
    {
        public float initialBPM
        {
            get => Core.RateToBPM(initialRate);
            set => initialRate = Core.BPMToRate(value);
        }
    }

    public class BPMChange
    {
        public float time;
        public float bpm;

        public BPMChange() { }
        public BPMChange(float time, float bpm)
        {
            this.time = time;
            this.bpm = bpm;
        }

        public BPMChange(LaneChange change)
        {
            time = change.time;
            bpm = Core.RateToBPM(change.rate);
        }

        public float GetRate() => Core.BPMToRate(bpm);

        public LaneChange ToChange()
        {
            return new LaneChange
            {
                time = time,
                rate = bpm / 60f
            };
        }
    }
}

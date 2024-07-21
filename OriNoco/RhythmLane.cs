namespace OriNoco
{
    public class RhythmLane : PredictableLane
    {
        public float initialBPM
        {
            get => 60f / initialRate;
            set => initialRate = 60f / value;
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
            bpm = 60f / change.rate;
        }

        public float GetRate() => 60f / bpm;

        public LaneChange ToChange()
        {
            return new LaneChange
            {
                time = time,
                rate = 60f / bpm
            };
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace OriNoco.Timing
{
    public class Metronome : MonoBehaviour
    {
        public static List<BeatRegion> regions = new() {
            new(new BeatTime(0), 120, 4)
        };

        public static float GetSecondsFromBeatTime(BeatTime time)
        {
            if (regions.Count == 0) return 0;
            if (regions.Count == 1) return GetSecondsFromBeat(time.whole, regions[0].beatPerMinute);

            float seconds = 0;
            float beat = 0;
            float bpm = regions[0].beatPerMinute;

            for (int i = 1; i < regions.Count; i++)
            {
                // If the time is still above the next region, add the seconds until the next region, then continue
                if (time >= regions[i].time)
                {
                    seconds += GetSecondsFromBeat(regions[i].time.whole - beat, bpm);
                    beat = regions[i].time.whole;
                    bpm = regions[i].beatPerMinute;
                }
                // If the time is not above the next region, return the seconds and stop
                else
                {
                    return seconds + GetSecondsFromBeat(time.whole - beat, bpm);
                }
            }

            // If we got here, it means we've gone through all the regions and we just need to add the seconds since the last region
            return seconds + GetSecondsFromBeat(time.whole - beat, bpm);
        }

        public static BeatTime GetBeatTimeFromSeconds(float seconds)
        {
            if (regions.Count == 0) return new BeatTime();
            if (regions.Count == 1) return new BeatTime(seconds * regions[0].beatPerMinute / 60f);

            float beat = 0;
            float time = 0;
            float bpm = regions[0].beatPerMinute;

            for (int i = 1; i < regions.Count; i++)
            {
                float secondsBeat = beat + GetBeatFromSeconds(seconds - time, bpm);
                if (secondsBeat >= regions[i].time.whole)
                {
                    time += GetSecondsFromBeat(regions[i].time.whole - beat, bpm);
                    beat = regions[i].time.whole;
                    bpm = regions[i].beatPerMinute;
                }
                else
                {
                    return new BeatTime(beat + GetBeatFromSeconds(seconds - time, bpm));
                }
            }

            return new BeatTime(beat + GetBeatFromSeconds(seconds - time, bpm));
        }

        private static float GetBeatFromSeconds(float seconds, float bpm) => seconds * bpm / 60f;
        private static float GetSecondsFromBeat(float beat, float bpm) => beat * 60f / bpm;

        public static BeatRegion GetRegionAtTime(BeatTime time)
        {
            if (time.whole < 0) return null;

            for (int i = 0; i < regions.Count; i++)
            {
                if (time < regions[i].time)
                    return regions[i - 1];
            }

            return regions[^1];
        }
    }
}
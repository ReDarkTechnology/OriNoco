using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriNoco.Timing
{
    public class Metronome : SingleInstance<Metronome>
    {
        private const float BeatTolerance = 0.000001f;
        public static readonly List<BeatRegion> regions = new() {
            new BeatRegion(new BeatTime(), 120, 4)
        };

        [Header("View Only")]
        public List<BeatRegion> viewRegions = new();

        private void Awake()
        {
            UpdateViewOnly();
        }

        public void UpdateViewOnly()
        {
            viewRegions = regions;
        }

        public static float GetSecondsFromBeatTime(BeatTime time)
        {
            switch (regions.Count)
            {
                case 0:
                    return 0;
                case 1:
                    return GetSecondsFromBeat(time.whole, regions[0].beatPerMinute);
            }

            float seconds = 0;
            float beat = 0;
            var bpm = regions[0].beatPerMinute;

            for (var i = 1; i < regions.Count; i++)
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
            switch (regions.Count)
            {
                case 0:
                    return new BeatTime();
                case 1:
                    return new BeatTime(seconds * regions[0].beatPerMinute / 60f);
            }

            float beat = 0;
            float time = 0;
            var bpm = regions[0].beatPerMinute;

            for (var i = 1; i < regions.Count; i++)
            {
                var secondsBeat = beat + GetBeatFromSeconds(seconds - time, bpm);
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

        public static BeatTime SnapBeatTimeToDivision(BeatTime time) => SnapBeatTimeToDivision(time, GetRegionAtTime(time).defaultSignature);
        /// <summary>
        /// Snaps a given <see cref="BeatTime"/> to a particular division.
        /// </summary>
        /// <param name="time">The time to snap.</param>
        /// <param name="divisor">The divisor to snap to.</param>
        /// <returns>The snapped <see cref="BeatTime"/>.</returns>
        /// <remarks>
        /// This method takes the fractional part of the <see cref="BeatTime"/>, multiplies it by the divisor, rounds to the nearest whole number, and then sets the numerator to that value and the denominator to the divisor.
        /// </remarks>
        public static BeatTime SnapBeatTimeToDivision(BeatTime time, float divisor)
        {
            var fractional = time.signature;
            var first = Mathf.Round(fractional * divisor);
            return new BeatTime(time.beat, first, divisor);
        }

        
        public static BeatTime GetNextBeat(BeatTime time) => GetNextBeat(time, GetRegionAtTime(time).defaultSignature);
        public static BeatTime GetNextBeat(BeatTime time, float divisor)
        {
            var snapped = SnapBeatTimeToDivision(time, divisor);
            snapped.numerator++;
            snapped.AdjustBeatAndFraction();
            return snapped;
        }
        
        public static BeatTime GetPreviousBeat(BeatTime time) => GetPreviousBeat(time, GetRegionAtTime(time).defaultSignature);
        public static BeatTime GetPreviousBeat(BeatTime time, float divisor)
        {
            var snapped = SnapBeatTimeToDivision(time, divisor);
            snapped.numerator--;
            snapped.AdjustBeatAndFraction();
            return snapped.whole < 0 ? new BeatTime() : snapped;
        }

        public static bool IsPartOfDivision(BeatTime time, float divisor)
        {
            var snapped = SnapBeatTimeToDivision(time, divisor);
            return Mathf.Abs(time.whole - snapped.whole) < BeatTolerance;
        }
        
        public static BeatRegion GetRegionAtTime(BeatTime time)
        {
            if (time.whole < 0) return regions[0];

            for (var i = 0; i < regions.Count; i++)
            {
                if (time < regions[i].time)
                    return regions[i - 1];
            }

            return regions[^1];
        }
    }
}
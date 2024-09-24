using System;
using UnityEngine;

namespace OriNoco.Timing
{
    public class TimeManager : SingleInstance<TimeManager>
    {
        private static BeatTime _time;
        public static BeatTime time
        {
            get => _time;
            set
            {
                _realtime = Metronome.GetSecondsFromBeatTime(value);
                _time = value;
                
                Instance?.UpdateViewOnly();
                onTimeChanged?.Invoke(value);
            }
        }

        private static float _realtime;
        public static float realtime
        {
            get => _realtime;
            set
            {
                _time = Metronome.GetBeatTimeFromSeconds(value);
                _realtime = value;
                
                Instance?.UpdateViewOnly();
                onRealtimeChanged?.Invoke(value);
            }
        }

        public static event Action<BeatTime> onTimeChanged;
        public static event Action<float> onRealtimeChanged;

        [Header("View Only")] 
        public BeatTime viewTime;
        public float viewRealtime;

        public void UpdateViewOnly()
        {
            viewTime = _time;
            viewRealtime = _realtime;
        }
    }
}
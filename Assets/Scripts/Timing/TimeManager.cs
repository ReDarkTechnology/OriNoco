using UnityEngine;

namespace OriNoco.Timing
{
    public class TimeManager : MonoBehaviour
    {
        private static BeatTime _time;
        public static BeatTime time
        {
            get => _time;
            set
            {
                _realtime = Metronome.GetSecondsFromBeatTime(value);
                _time = value;
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
            }
        }
    }
}
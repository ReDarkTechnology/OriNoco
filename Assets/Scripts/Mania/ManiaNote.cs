using UnityEngine;
using OriNoco.Timing;

namespace OriNoco.Mania
{
    public class ManiaNote : MonoBehaviour
    {
        [Header("Properties")]
        public BeatTime time = new();
        public NoteType type = NoteType.Tap;
        public Direction direction = Direction.Up;

        [Header("Graphics")]
        public RectTransform startNote;
        public RectTransform endNote;

        private float cachedSeconds;

        public void UpdateTimeCache()
        {
            cachedSeconds = Metronome.GetSecondsFromBeatTime(time);
        }
    }
}
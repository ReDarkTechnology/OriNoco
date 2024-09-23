using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriNoco.Timing
{
    public class TimeManager : MonoBehaviour
    {
        public BeatTime time;
        private Metronome metronome;

        public float fTime
        {
            get => metronome.GetSecondsFromBeatTime(time);
            set => time = metronome.GetBeatTimeFromSeconds(value);
        }

        private void Start()
        {
            metronome = References.Get<Metronome>();
        }

        private void Update()
        {
        }
    }
}
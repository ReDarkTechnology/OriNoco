using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriNoco.Timing
{
    [ExecuteInEditMode]
    public class TimeManager : MonoBehaviour
    {
        public BeatTime time;
        private Metronome metronome;

        private void Start()
        {
            metronome = References.Get<Metronome>();
        }

        private void Update()
        {
            if (Application.isPlaying) UpdateGame(); else UpdateEditor();
            UpdateHybrid();
        }

        private void UpdateEditor()
        {

        }

        private void UpdateGame()
        {

        }

        private void UpdateHybrid()
        {

        }
    }
}
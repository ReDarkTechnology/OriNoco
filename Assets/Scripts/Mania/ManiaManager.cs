using OriNoco.Timing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriNoco.Mania
{
    public class ManiaManager : SingleInstance<ManiaManager>
    {
        public static List<ManiaNote> notes = new();
        
        [Range(1f, 10000f)]
        public float scale = 100f;
        
        public GameObject notePrefab;
        public NoteType noteType = NoteType.Tap;
        public Sprite[] noteSprites;
        public Color[] noteColors = new []
        {
            Color.cyan, Color.green, Color.red, Color.yellow, 
        };
        public Color[] noteTypeColors = new []
        {
            Color.white, Color.blue, Color.white, Color.white, Color.magenta  
        };
        
        public List<ManiaLane> lanes = new();
        public RectTransform containerRect;
        public RectTransform fullRect;

        public int yOffset;

        public float GetSecondsFromScreenPoint(float y)
        {
            return (y - yOffset - containerRect.anchoredPosition.y) / scale;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            // Scrolling
            if (Input.GetKeyDown(KeyCode.W))
                ScrollNext();
            if (Input.GetKeyDown(KeyCode.S))
                ScrollPrevious();
            
            if (Input.mouseScrollDelta.y < 0)
                ScrollNext();
            else if (Input.mouseScrollDelta.y > 0)
                ScrollPrevious();

            // Note creation
            if (Input.GetKeyDown(KeyCode.LeftArrow)) CreateNoteAtLane(0);
            if (Input.GetKeyDown(KeyCode.DownArrow)) CreateNoteAtLane(1);
            if (Input.GetKeyDown(KeyCode.UpArrow)) CreateNoteAtLane(2);
            if (Input.GetKeyDown(KeyCode.RightArrow)) CreateNoteAtLane(3);
        }

        private void CreateNoteAtLane(int index)
        {
            lanes[index].CreateNote(TimeManager.time, noteType);
        }

        /// <summary>
        /// Scrolls to the given <see cref="BeatTime"/>. This will set the current time and call <see cref="UpdateRenderer"/>.
        /// </summary>
        /// <param name="time">The time to scroll to.</param>
        public void ScrollTo(BeatTime time)
        {
            TimeManager.time = time;
            UpdateRenderer();
        }

        /// <summary>
        /// Scrolls to the next beat.
        /// </summary>
        /// <remarks>
        /// This will scroll to the next beat in the current time signature.
        /// </remarks>
        public void ScrollNext() => ScrollTo(Metronome.GetNextBeat(TimeManager.time));
        
        /// <summary>
        /// Scrolls to the previous beat.
        /// </summary>
        /// <remarks>
        /// This will scroll to the previous beat in the current time signature.
        /// </remarks>
        public void ScrollPrevious() => ScrollTo(Metronome.GetPreviousBeat(TimeManager.time));

        public void UpdateRenderer()
        {
            var pos = containerRect.anchoredPosition;
            pos.y = -Metronome.GetSecondsFromBeatTime(TimeManager.time) * scale;
            containerRect.anchoredPosition = pos;
        }
    }
}
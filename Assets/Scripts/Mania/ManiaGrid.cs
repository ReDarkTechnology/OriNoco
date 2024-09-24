using System;
using System.Collections.Generic;
using OriNoco.Timing;
using UnityEngine;
using UnityEngine.UI;

namespace OriNoco.Mania
{
    public class ManiaGrid : MonoBehaviour
    {
        public RectTransform containerRect;
        public GameObject linePrefab;

        public Color onBeatColor = Color.cyan;
        public Color inBetweenColor = Color.gray;

        public List<Image> lines = new();
        public int lineLimit = 250;

        /// <summary>
        /// Initializes the grid and listens for time changes.
        /// </summary>
        private void Start()
        {
            UpdateGrid();
            TimeManager.onTimeChanged += time => UpdateGrid();
        }


        /// <summary>
        /// Updates the grid to the current time and beat region.
        /// </summary>
        /// <remarks>
        /// This will instantiate and position lines until the line limit is reached, and then destroy any lines that are no longer required.
        /// </remarks>
        private void UpdateGrid()
        {
            if (!containerRect)
                return;

            var currentBeat = TimeManager.time;            
            var i = 0;
            var limitY = containerRect.rect.height + Metronome.GetSecondsFromBeatTime(currentBeat) * ManiaManager.Instance.scale;
            
            while (i < lineLimit)
            {
                var targetPosition = new Vector2(0, Metronome.GetSecondsFromBeatTime(currentBeat) * ManiaManager.Instance.scale);
                if (targetPosition.y > limitY)
                    break;

                if (i < lines.Count)
                {
                    var line = lines[i]; 
                    var lineRect = ((RectTransform)line.transform);
                    lineRect.anchoredPosition = targetPosition;
                    
                    var s = lineRect.sizeDelta;
                    s.x = containerRect.rect.width;
                    lineRect.sizeDelta = s;
                    
                    line.color = currentBeat.signature == 0 ? onBeatColor : inBetweenColor;
                }
                else
                {
                    var line = Instantiate(linePrefab, transform).GetComponent<Image>();
                    var lineRect = ((RectTransform)line.transform);
                    lineRect.anchoredPosition = targetPosition;
                    
                    var s = lineRect.sizeDelta;
                    s.x = containerRect.rect.width;
                    lineRect.sizeDelta = s;
                    line.color = currentBeat.signature == 0 ? onBeatColor : inBetweenColor;
                    lines.Add(line);
                }
                currentBeat = Metronome.GetNextBeat(currentBeat, Metronome.GetRegionAtTime(currentBeat).defaultSignature);
                i++;
            }

            for (var j = i; j < lines.Count; j++)
            {
                if (lines[j])
                    Destroy(lines[j].gameObject);
            }
            
            lines.RemoveRange(i, lines.Count - i);
        }
    }   
}

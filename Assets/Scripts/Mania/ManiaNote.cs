using UnityEngine;
using OriNoco.Timing;
using UnityEngine.UI;

namespace OriNoco.Mania
{
    public class ManiaNote : MonoBehaviour
    {
        [Header("Properties")]
        public BeatTime time = new();
        public NoteType type = NoteType.Tap;
        public Direction direction = Direction.Up;

        [Header("Graphics")]
        public Image noteImage;

        /// <summary>
        /// Updates the graphics of the note, including its position and sprite.
        /// </summary>
        /// <remarks>
        /// This should be called whenever the note's time, direction, or type changes.
        /// </remarks>
        public void UpdateGraphics()
        {
            var rectTransform = (RectTransform)transform;
            var pos = rectTransform.anchoredPosition;
            pos.y = Metronome.GetSecondsFromBeatTime(time) * ManiaManager.Instance.scale;
            rectTransform.anchoredPosition = pos;
            noteImage.sprite = ManiaManager.Instance.noteSprites[(int)direction];
            noteImage.color = ManiaManager.Instance.noteColors[(int)direction] * ManiaManager.Instance.noteTypeColors[(int)type]; 
        }
    }
}
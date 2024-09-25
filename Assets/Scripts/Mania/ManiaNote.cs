using UnityEngine;
using OriNoco.Timing;
using UnityEngine.UI;

namespace OriNoco.Mania
{
    public class ManiaNote : MonoBehaviour
    {
        [Header("Properties")]
        public BeatTime time = new();
        public BeatTime endTime = new();
        public NoteType type = NoteType.Tap;
        public Direction direction = Direction.Up;

        [Header("Graphics")]
        public Image noteImage;
        public Image middleImage;
        public Image endImage;

        /// <summary>
        /// Updates the graphics of the note, including its position and sprite.
        /// </summary>
        /// <remarks>
        /// This should be called whenever the note's time, direction, or type changes.
        /// </remarks>
        public void UpdateGraphics()
        {
            var color = ManiaManager.Instance.noteColors[(int)direction] * ManiaManager.Instance.noteTypeColors[(int)type];

            float start = Metronome.GetSecondsFromBeatTime(time);
            float end = Metronome.GetSecondsFromBeatTime(endTime);

            if (start > end)
            {
                (time, endTime) = (endTime, time);
                (start, end) = (end, start);
            }

            var rectTransform = (RectTransform)transform;
            var pos = rectTransform.anchoredPosition;
            pos.y = start * ManiaManager.Instance.scale;
            rectTransform.anchoredPosition = pos;

            noteImage.sprite = ManiaManager.Instance.noteSprites[(int)direction];
            noteImage.color = color;

            if (start != end)
            {
                middleImage.gameObject.SetActive(true);
                endImage.gameObject.SetActive(true);

                color.a = 0.2f;
                middleImage.color = color;
                endImage.color = color;

                var mRectTransform = (RectTransform)middleImage.transform;
                var mScale = mRectTransform.sizeDelta;
                mScale.y = (end - start) * ManiaManager.Instance.scale;
                mRectTransform.sizeDelta = mScale;
                var mPos = mRectTransform.anchoredPosition;
                mPos.y = ((end - start) / 2) * ManiaManager.Instance.scale;
                mRectTransform.anchoredPosition = mPos;

                var eRectTransform = (RectTransform)endImage.transform;
                var ePos = eRectTransform.anchoredPosition;
                ePos.y = (end - start) * ManiaManager.Instance.scale;
                eRectTransform.anchoredPosition = ePos;
            }
            else
            {
                middleImage.gameObject.SetActive(false);
                endImage.gameObject.SetActive(false);
            }
        }
    }
}
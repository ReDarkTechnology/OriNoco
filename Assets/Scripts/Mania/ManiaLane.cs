using System;
using System.Collections.Generic;
using UnityEngine;
using OriNoco.Timing;
using UnityEngine.UI;

namespace OriNoco.Mania
{
    public class ManiaLane : MonoBehaviour
    {
        public RectTransform notesParent;
        public RectTransform referencePoint;

        public Direction direction = Direction.Up;
        public List<ManiaNote> notes = new();

        public Image image;

        public Image previewStart;
        public Image previewMiddle;
        public Image previewEnd;

        private RectTransform _rectTransform;
        private RectTransform _container;
        private RectTransform _rPreviewStart;
        private RectTransform _rPreviewMiddle;
        private RectTransform _rPreviewEnd;

        private bool _wasHovering;
        private BeatTime _startCreateTime;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;

            _rPreviewStart = previewStart.transform as RectTransform;
            _rPreviewMiddle = previewMiddle.transform as RectTransform;
            _rPreviewEnd = previewEnd.transform as RectTransform;
        }

        private void Start()
        {
            _container = ManiaManager.Instance.fullRect;

            previewStart.sprite = ManiaManager.Instance.noteSprites[(int)direction];
            var color = ManiaManager.Instance.noteColors[(int)direction];
            color.a = 0.05f;

            previewStart.color = color;
            previewMiddle.color = color;
            previewEnd.color = color;
        }

        private void Update()
        {
            bool isInside = IsInsideRect(Input.mousePosition, out Vector2 pos);
            bool isAllowedAdjusting = (isInside && !ManiaManager.creatingLane) || ManiaManager.creatingLane == this;
            if (isAllowedAdjusting)
            {
                if (!_wasHovering)
                {
                    OnHoverChanged(true);
                    _wasHovering = true;
                }

                var snappedBeatTime = Metronome.SnapBeatTimeToDivision(
                    Metronome.GetBeatTimeFromSeconds(
                        ManiaManager.Instance.GetSecondsFromScreenPoint(pos.y)
                    )
                );

                if (!ManiaManager.creatingLane)
                {
                    var position = _rPreviewStart.anchoredPosition;
                    position.y = Metronome.GetSecondsFromBeatTime(snappedBeatTime) * ManiaManager.Instance.scale;
                    _rPreviewStart.anchoredPosition = position;

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        ManiaManager.creatingLane = this;
                        _startCreateTime = snappedBeatTime;
                    }
                }
                else if (ManiaManager.creatingLane == this)
                {
                    if (_startCreateTime != snappedBeatTime)
                    {
                        previewMiddle.gameObject.SetActive(true);
                        previewEnd.gameObject.SetActive(true);
                        AdjustPreview(Metronome.GetSecondsFromBeatTime(_startCreateTime), Metronome.GetSecondsFromBeatTime(snappedBeatTime));
                    }
                    else
                    {
                        previewMiddle.gameObject.SetActive(false);
                        previewEnd.gameObject.SetActive(false);
                    }

                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        if (_startCreateTime != snappedBeatTime)
                        {
                            CreateNote(_startCreateTime, snappedBeatTime);
                        }
                        else
                        {
                            CreateNote(snappedBeatTime, ManiaManager.Instance.noteType);
                        }

                        ManiaManager.creatingLane = null;

                        previewMiddle.gameObject.SetActive(false);
                        previewEnd.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (_wasHovering)
                {
                    OnHoverChanged(false);
                    _wasHovering = false;
                }
            }
        }

        public void AdjustPreview(float start, float end)
        {
            if (start > end)
                (start, end) = (end, start);

            var startPos = _rPreviewStart.anchoredPosition;
            startPos.y = start * ManiaManager.Instance.scale;
            _rPreviewStart.anchoredPosition = startPos;

            var middlePos = _rPreviewMiddle.anchoredPosition;
            middlePos.y = (((end - start) / 2) + start) * ManiaManager.Instance.scale;
            _rPreviewMiddle.anchoredPosition = middlePos;
            var middleScale = _rPreviewMiddle.sizeDelta;
            middleScale.y = (end - start) * ManiaManager.Instance.scale;
            _rPreviewMiddle.sizeDelta = middleScale;

            var endPos = _rPreviewEnd.anchoredPosition;
            endPos.y = end * ManiaManager.Instance.scale;
            _rPreviewEnd.anchoredPosition = endPos;
        }

        private bool IsInsideRect(Vector2 screenPoint, out Vector2 localPosition)
        {
            var halfWidth = _rectTransform.rect.width / 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(referencePoint, screenPoint,
                    null, out var pos);
            localPosition = pos;
            return pos.x < halfWidth && pos.x > -halfWidth && pos.y > 0 && pos.y < _container.rect.height;
        }

        private void OnHoverChanged(bool to)
        {
            previewStart.gameObject.SetActive(to);
            
            var col = image.color;
            col.a = to ? 0.01f : 0f;
            image.color = col;
        }
        
        public void InitiateNoteCreate()
        {

        }

        /// <summary>
        /// Creates a new note at the given time and type, and parents it to this lane.
        /// </summary>
        /// <param name="time">The time of the note.</param>
        /// <param name="type">The type of the note.</param>
        /// <returns>The created note.</returns>
        public ManiaNote CreateNote(BeatTime time, NoteType type)
        {
            var obj = Instantiate(ManiaManager.Instance.notePrefab, notesParent);
            var note = obj.GetComponent<ManiaNote>();
            (note.direction, note.time, note.endTime, note.type) = (direction, time, time, type);
            note.UpdateGraphics();
            return note;
        }

        /// <summary>
        /// Creates a new note at the given time and type, and parents it to this lane.
        /// </summary>
        /// <param name="time">The time of the note.</param>
        /// <param name="type">The type of the note.</param>
        /// <returns>The created note.</returns>
        public ManiaNote CreateNote(BeatTime time, BeatTime endTime)
        {
            var obj = Instantiate(ManiaManager.Instance.notePrefab, notesParent);
            var note = obj.GetComponent<ManiaNote>();
            (note.direction, note.time, note.endTime, note.type) = (direction, time, endTime, NoteType.Hold);
            note.UpdateGraphics();
            return note;
        }
    }
}
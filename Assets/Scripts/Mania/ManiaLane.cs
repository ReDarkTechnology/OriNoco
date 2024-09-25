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
        public Image previewNote;
        
        private RectTransform _rectTransform;
        private RectTransform _container;
        private RectTransform _previewNoteRect;
        
        private bool _wasHovering;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _previewNoteRect = previewNote.transform as RectTransform;
        }

        private void Start()
        {
            _container = ManiaManager.Instance.fullRect;
            previewNote.sprite = ManiaManager.Instance.noteSprites[(int)direction];
        }

        private void Update()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(referencePoint, Input.mousePosition,
                    null, out var pos);
            
            var halfWidth = _rectTransform.rect.width / 2;
            if (pos.x < halfWidth && pos.x > -halfWidth && pos.y > 0 && pos.y < _container.rect.height)
            {
                if (!_wasHovering)
                {
                    OnHoverChanged(true);
                    _wasHovering = true;
                }

                var position = _previewNoteRect.anchoredPosition;
                var snappedBeatTime = Metronome.SnapBeatTimeToDivision(
                    Metronome.GetBeatTimeFromSeconds(
                        ManiaManager.Instance.GetSecondsFromScreenPoint(pos.y)
                    )
                );
                position.y = Metronome.GetSecondsFromBeatTime(snappedBeatTime) * ManiaManager.Instance.scale;
                _previewNoteRect.anchoredPosition = position;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    CreateNote(snappedBeatTime, ManiaManager.Instance.noteType);
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

        private void OnHoverChanged(bool to)
        {
            previewNote.gameObject.SetActive(to);
            var color = ManiaManager.Instance.noteColors[(int)direction];
            color.a = 0.05f;
            previewNote.color = color;
            
            var col = image.color;
            col.a = to ? 0.01f : 0f;
            image.color = col;
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
            (note.direction, note.time, note.type) = (direction, time, type);
            note.UpdateGraphics();
            return note;
        }
    }
}
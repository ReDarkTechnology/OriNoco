using System.Collections.Generic;
using UnityEngine;
using OriNoco.Timing;

namespace OriNoco.Mania
{
    public class ManiaLane : MonoBehaviour
    {
        public Direction direction = Direction.Up;
        public List<ManiaNote> notes = new();

        /// <summary>
        /// Creates a new note at the given time and type, and parents it to this lane.
        /// </summary>
        /// <param name="time">The time of the note.</param>
        /// <param name="type">The type of the note.</param>
        /// <returns>The created note.</returns>
        public ManiaNote CreateNote(BeatTime time, NoteType type)
        {
            var obj = Instantiate(ManiaManager.Instance.notePrefab, transform);
            var note = obj.GetComponent<ManiaNote>();
            (note.direction, note.time, note.type) = (direction, time, type);
            note.UpdateGraphics();
            return note;
        }
    }
}
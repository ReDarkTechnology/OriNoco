using System.Collections.Generic;
using UnityEngine;
using OriNoco.Timing;

namespace OriNoco.Mania
{
    public class ManiaLane : MonoBehaviour
    {
        public Direction direction = Direction.Up;
        public List<ManiaNote> notes = new();

        public ManiaNote CreateNote(BeatTime time, NoteType type)
        {
            var obj = Instantiate(ManiaManager.Instance.notePrefab, transform);
            var note = obj.GetComponent<ManiaNote>();
            (note.time, note.type) = (time, type);
            note.UpdateTimeCache();
            return note;
        }
    }
}
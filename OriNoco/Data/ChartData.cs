using System;
using System.Collections.Generic;

namespace OriNoco.Data
{
    [Serializable]
    public class ChartData
    {
        public ChartInfoData info = new();
        public List<NoteData> notes = [];
    }
}

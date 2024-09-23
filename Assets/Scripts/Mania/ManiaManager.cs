using OriNoco.Timing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriNoco.Mania
{
    public class ManiaManager : SingleInstance<ManiaManager>
    {
        public GameObject notePrefab;

        public List<ManiaLane> lanes = new();
        public static List<ManiaNote> notes = new();

        public RectTransform containerRect;
        public float scale;

        private void Awake() => Instance = this;
        private void Update()
        {
            // Scrolling

            // Note creation

        }

        public void UpdateRenderer()
        {

        }
    }
}
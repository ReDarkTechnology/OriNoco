using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriNoco.Theme
{
    [CreateAssetMenu(fileName = "ThemeDictionary", menuName = "OriNoco/ThemeDictionary", order = 1)]
    [Serializable]
    public class ThemeDictionary : ScriptableObject
    {
        public List<ThemeItem> items = new()
        {
            new("Background", new Color32(38, 38, 38, 255)),
            new("BackgroundDark", new Color32(13, 13, 13, 255)),
            new("BackgroundLight", new Color32(64, 64, 64, 255)),
            new("Foreground", Color.white),
            new("ForegroundDark", new Color32(200, 200, 200, 255)),
            new("Outline", new Color32(83, 83, 83, 255)),
        };

        public ThemeItem GetItem(string name) => items.Find(x => x.name == name);
    }

    [Serializable]
    public class ThemeItem
    {
        public string name;
        public Sprite background;
        public Color color;

        public ThemeItem() { }
        public ThemeItem(string name) : this(name, null, Color.white) { }
        public ThemeItem(string name, Color color) : this(name, null, color) { }
        public ThemeItem(string name, Sprite background) : this(name, background, Color.white) { }
        public ThemeItem(string name, Sprite background, Color color)
        {
            this.name = name;
            this.background = background;
            this.color = color;
        }
    }
}

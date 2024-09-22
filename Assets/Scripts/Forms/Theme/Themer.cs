using System;
using UnityEngine;

namespace OriNoco.Theme
{
    public class Themer : MonoBehaviour
    {
        [SerializeField]
        private ThemeDictionary initialTheme;

        public static ThemeDictionary currentTheme { get; private set; }
        public static event Action<ThemeDictionary> onThemeChange;

        public Themer()
        {
            currentTheme = initialTheme;
        }

        public void SetTheme(ThemeDictionary theme)
        {
            currentTheme = theme;
            onThemeChange?.Invoke(theme);
        }
    }
}

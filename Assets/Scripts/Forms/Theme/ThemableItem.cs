using UnityEngine;

namespace OriNoco.Theme
{
    public class ThemableItem : MonoBehaviour
    {
        public virtual void Start()
        {
            Themer.onThemeChange += SetTheme;
            SetTheme(Themer.currentTheme);

            Debug.Log("Theme listener called!");
        }

        public virtual void SetTheme(ThemeDictionary dictionary)
        {

        }
    }
}
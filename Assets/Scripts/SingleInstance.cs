using UnityEngine;

namespace OriNoco
{
    public class SingleInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _Instance;
        public static T Instance
        {
            get => _Instance ??= FindObjectOfType<T>(true);
            set => _Instance = value;
        }
    }
}

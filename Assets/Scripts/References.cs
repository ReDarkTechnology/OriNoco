using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriNoco
{
    public class References : MonoBehaviour
    {
        private static References _instance;
        public static References Instance => _instance ??= FindObjectOfType<References>(true);

        [SerializeField]
        private List<MonoBehaviour> objects = new();
        private static readonly Dictionary<string, MonoBehaviour> refs = new();

        private bool _wasInitialized;

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Multiple References found!");
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public void Initialize()
        {
            if (_wasInitialized) return;
            foreach (MonoBehaviour obj in objects)
                refs.Add(obj.GetType().Name, obj);
            _wasInitialized = true;
        }

        public static T Get<T>(string name) where T : MonoBehaviour
        {
            Instance.Initialize();

            if (refs.ContainsKey(name)) return refs[name] as T;
            T t = Instance.transform.Find(name).GetComponent<T>();
            refs.Add(name, t);
            return t;
        }

        public static T Get<T>() where T : MonoBehaviour
        {
            Instance.Initialize();

            string name = typeof(T).Name;
            if (refs.ContainsKey(name)) return refs[name] as T;
            T t = FindObjectOfType<T>(true);
            refs.Add(name, t);
            return t;
        }
    }
}
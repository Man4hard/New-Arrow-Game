// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Generic scene-scoped singleton base for MonoBehaviours.
    /// Only one instance is allowed per scene. Duplicates are destroyed automatically.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogError($"[MonoSingleton] No instance of {typeof(T).Name} found in scene.");
                return _instance;
            }
        }

        public static bool Exists => _instance != null;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"[MonoSingleton] Duplicate {typeof(T).Name} destroyed.");
                Destroy(gameObject);
                return;
            }
            _instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}

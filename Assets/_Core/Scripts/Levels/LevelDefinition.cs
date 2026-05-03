// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Represents a single level. Acts as both a data container (prefab reference)
    /// and a runtime controller once instantiated.
    /// Attach this to every level prefab root.
    /// </summary>
    public class LevelDefinition : MonoBehaviour
    {
        [Header("Level Info")]
        [SerializeField] string _levelName = "Level";

        public string LevelName => _levelName;

        public event Action OnLevelReady;

        public void Activate()
        {
            gameObject.SetActive(true);
            OnLevelReady?.Invoke();
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}

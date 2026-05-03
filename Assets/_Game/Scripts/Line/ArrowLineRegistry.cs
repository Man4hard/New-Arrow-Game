// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Keeps track of all active ArrowLine objects in the current level.
    /// When the last one is cleared, fires OnAllCleared which the win-condition
    /// system listens to.
    /// Attach once to a persistent manager object or the level root.
    /// </summary>
    public class ArrowLineRegistry : MonoBehaviour
    {
        [SerializeField] ArrayPoolProvider _arrayPool;

        readonly List<ArrowLine> _active = new();

        public IReadOnlyList<ArrowLine> Active => _active;
        public int Count => _active.Count;
        public ArrayPoolProvider ArrayPool => _arrayPool;

        public event Action OnAllCleared;

        /// <summary>
        /// Finds every ArrowLine under levelRoot and calls Setup on each one.
        /// </summary>
        public void InitialiseLevel(Transform levelRoot)
        {
            ClearAll();

            if (levelRoot == null)
            {
                Debug.LogWarning("[ArrowLineRegistry] levelRoot is null.");
                return;
            }

            ArrowLine[] lines = levelRoot.GetComponentsInChildren<ArrowLine>(includeInactive: true);
            foreach (var line in lines)
                line.Setup(this);
        }

        public void Register(ArrowLine line)
        {
            if (line != null && !_active.Contains(line))
                _active.Add(line);
        }

        public void Unregister(ArrowLine line)
        {
            if (line == null) return;
            _active.Remove(line);

            if (_active.Count == 0)
                OnAllCleared?.Invoke();
        }

        public void ClearAll()
        {
            foreach (var line in _active)
                line?.Cleanup();
            _active.Clear();
        }
    }
}

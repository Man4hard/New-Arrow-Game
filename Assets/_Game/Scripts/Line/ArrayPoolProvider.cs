// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Lightweight pool for Vector3 arrays used by animation code.
    /// Implemented as a plain Stack — no MonoBehaviour wrappers needed.
    /// Arrays larger than <see cref="_maxLength"/> are allocated fresh each time
    /// and not pooled (they're rare edge cases not worth caching).
    /// </summary>
    public class ArrayPoolProvider : MonoBehaviour
    {
        [SerializeField] int _maxLength    = 128;
        [SerializeField] int _initialCount = 8;

        readonly Stack<Vector3[]> _pool = new();

        void Awake()
        {
            for (int i = 0; i < _initialCount; i++)
                _pool.Push(new Vector3[_maxLength]);
        }

        /// <summary>Rent an array of at least <paramref name="length"/> elements.</summary>
        public Vector3[] Rent(int length)
        {
            if (length > _maxLength)
                return new Vector3[length]; // too large to pool

            if (_pool.Count > 0)
                return _pool.Pop();

            return new Vector3[_maxLength];
        }

        /// <summary>Return a previously rented array to the pool.</summary>
        public void Return(Vector3[] array)
        {
            if (array == null || array.Length > _maxLength) return;
            _pool.Push(array);
        }
    }
}

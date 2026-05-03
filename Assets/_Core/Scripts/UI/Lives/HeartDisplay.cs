// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Subscribes to LifeTracker and updates the row of HeartIcon components.
    /// Assign exactly as many HeartIcon children in the Inspector as _maxLives.
    /// </summary>
    public class HeartDisplay : MonoBehaviour
    {
        [SerializeField] List<HeartIcon> _icons = new();

        void OnEnable()
        {
            if (LifeTracker.Exists)
            {
                LifeTracker.Instance.OnLivesChanged += Refresh;
                Refresh(LifeTracker.Instance.Current);
            }
        }

        void OnDisable()
        {
            if (LifeTracker.Exists)
                LifeTracker.Instance.OnLivesChanged -= Refresh;
        }

        void Refresh(int livesRemaining)
        {
            for (int i = 0; i < _icons.Count; i++)
            {
                if (_icons[i] != null)
                    _icons[i].SetState(i < livesRemaining);
            }
        }
    }
}

// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Tracks the player's remaining lives for a single round.
    /// Emits events so HeartDisplay and any GameOver logic can react.
    /// Frame-duplicate guard uses a small time window instead of
    /// frame equality to handle multi-collision edge cases reliably.
    /// </summary>
    public class LifeTracker : MonoSingleton<LifeTracker>
    {
        [Header("Settings")]
        [SerializeField][Range(1, 10)] int _maxLives = 5;
        [SerializeField][Range(0.05f, 1f)] float _lossCooldown = 0.15f;

        int   _lives;
        float _lastLossTime = -999f;

        public int Current  => _lives;
        public int Maximum  => _maxLives;
        public bool HasLives => _lives > 0;

        public event Action<int> OnLivesChanged;  // passes new count
        public event Action      OnAllLivesLost;

        protected override void Awake()
        {
            base.Awake();
            Reset();
        }

        public void Reset()
        {
            _lives        = _maxLives;
            _lastLossTime = -999f;
            OnLivesChanged?.Invoke(_lives);
        }

        /// <summary>
        /// Deduct one life. Calls within the cooldown window are ignored
        /// to prevent a single collision event from being processed twice.
        /// </summary>
        public void LoseOne()
        {
            float now = Time.unscaledTime;
            if (now - _lastLossTime < _lossCooldown) return;
            if (_lives <= 0) return;

            _lastLossTime = now;
            _lives--;
            OnLivesChanged?.Invoke(_lives);

            if (_lives <= 0)
                OnAllLivesLost?.Invoke();
        }
    }
}

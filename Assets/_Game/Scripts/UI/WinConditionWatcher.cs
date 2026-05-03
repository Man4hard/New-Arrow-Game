// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Listens to the ArrowLineRegistry and LifeTracker.
    /// Triggers Win or Lose on the GameStateController accordingly.
    /// Place once in the scene alongside the registry.
    /// </summary>
    public class WinConditionWatcher : MonoBehaviour
    {
        [SerializeField] ArrowLineRegistry _registry;

        void OnEnable()
        {
            if (_registry != null)
                _registry.OnAllCleared += HandleAllCleared;

            if (Core.LifeTracker.Exists)
                Core.LifeTracker.Instance.OnAllLivesLost += HandleLivesLost;
        }

        void OnDisable()
        {
            if (_registry != null)
                _registry.OnAllCleared -= HandleAllCleared;

            if (Core.LifeTracker.Exists)
                Core.LifeTracker.Instance.OnAllLivesLost -= HandleLivesLost;
        }

        void HandleAllCleared()
        {
            if (!Core.GameStateController.Exists) return;
            if (!Core.GameStateController.Instance.IsPlaying) return;

            Core.GameStateController.Instance.SetWon();
        }

        void HandleLivesLost()
        {
            if (!Core.GameStateController.Exists) return;
            if (!Core.GameStateController.Instance.IsPlaying) return;

            Core.GameStateController.Instance.SetLost();
        }
    }
}

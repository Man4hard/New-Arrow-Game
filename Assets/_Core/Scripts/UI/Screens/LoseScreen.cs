// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Shown when the player runs out of lives.
    /// Retry restarts the same level; lives are reset.
    /// </summary>
    public class LoseScreen : UIScreen
    {
        [SerializeField] Button          _retryButton;
        [SerializeField] TextMeshProUGUI _messageLabel;

        protected override void Awake()
        {
            base.Awake();
            _retryButton?.onClick.AddListener(OnRetryClicked);
            Hide();
        }

        void OnEnable()
        {
            if (GameStateController.Exists)
                GameStateController.Instance.OnStateChanged += HandleStateChanged;
        }

        void OnDisable()
        {
            if (GameStateController.Exists)
                GameStateController.Instance.OnStateChanged -= HandleStateChanged;
        }

        void HandleStateChanged(GameState from, GameState to)
        {
            if (to == GameState.Lost) Show();
        }

        void OnRetryClicked()
        {
            Hide();
            LifeTracker.Instance?.Reset();
            LevelController.Instance?.RetryCurrentLevel();
        }

        protected override void OnShow()
        {
            if (_messageLabel != null)
                _messageLabel.text = "Try Again!";
        }
    }
}

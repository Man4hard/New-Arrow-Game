// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using TMPro;
using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// In-game heads-up display. Shows level number and hearts.
    /// Subscribes to GameStateController to auto-show/hide.
    /// </summary>
    public class HUDScreen : UIScreen
    {
        [SerializeField] TextMeshProUGUI _levelLabel;

        protected override void Awake()
        {
            base.Awake();
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

        public void SetLevel(int number)
        {
            if (_levelLabel != null)
                _levelLabel.text = $"Level {number}";
        }

        void HandleStateChanged(GameState from, GameState to)
        {
            if (to == GameState.Playing) Show();
            else if (to == GameState.Won || to == GameState.Lost) Hide();
        }
    }
}

// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Shown when the player clears all arrows.
    /// The "Next" button advances to the next level.
    /// </summary>
    public class WinScreen : UIScreen
    {
        [SerializeField] Button          _nextButton;
        [SerializeField] TextMeshProUGUI _congratsLabel;

        protected override void Awake()
        {
            base.Awake();
            _nextButton?.onClick.AddListener(OnNextClicked);
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
            if (to == GameState.Won) Show();
        }

        void OnNextClicked()
        {
            Hide();
            LevelController.Instance?.AdvanceToNextLevel();
        }

        protected override void OnShow()
        {
            if (_congratsLabel != null)
                _congratsLabel.text = "Well Done!";
        }
    }
}

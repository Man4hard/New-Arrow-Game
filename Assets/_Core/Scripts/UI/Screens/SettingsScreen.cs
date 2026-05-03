// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;
using UnityEngine.UI;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Simple settings overlay with sound toggle.
    /// The close button hides the panel and resumes play.
    /// </summary>
    public class SettingsScreen : UIScreen
    {
        [SerializeField] Button _closeButton;
        [SerializeField] Toggle _soundToggle;

        protected override void Awake()
        {
            base.Awake();
            _closeButton?.onClick.AddListener(OnCloseClicked);

            if (_soundToggle != null)
            {
                bool on = SaveSystem.GetInt(PrefKeys.SoundEnabled, 1) == 1;
                _soundToggle.isOn = on;
                _soundToggle.onValueChanged.AddListener(OnSoundToggled);
            }

            Hide();
        }

        void OnCloseClicked()
        {
            Hide();
            GameStateController.Instance?.SetPlaying();
        }

        void OnSoundToggled(bool isOn)
        {
            SoundManager.Instance?.SetSfxEnabled(isOn);
        }
    }
}

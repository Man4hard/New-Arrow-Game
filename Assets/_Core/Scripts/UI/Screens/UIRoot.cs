// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Central hub that wires up all UI screens.
    /// Each screen is self-managing via GameStateController events,
    /// so this class simply holds references and exposes
    /// convenience methods for buttons that need to open specific panels.
    /// </summary>
    public class UIRoot : MonoSingleton<UIRoot>
    {
        [Header("Screens")]
        [SerializeField] HUDScreen      _hud;
        [SerializeField] WinScreen      _win;
        [SerializeField] LoseScreen     _lose;
        [SerializeField] SettingsScreen _settings;

        public void OpenSettings()
        {
            GameStateController.Instance?.SetPaused();
            _settings?.Show();
        }
    }
}

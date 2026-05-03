// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Central state machine for the game session.
    /// Fires events on every transition so other systems can react
    /// without coupling to each other directly.
    /// </summary>
    [DefaultExecutionOrder(-5)]
    public class GameStateController : MonoSingleton<GameStateController>
    {
        [SerializeField] GameState _initialState = GameState.Loading;

        GameState _current = GameState.None;

        public GameState Current => _current;

        public event Action<GameState, GameState> OnStateChanged; // (from, to)

        protected override void Awake()
        {
            base.Awake();
            Transition(_initialState);
        }

        public void SetPlaying() => Transition(GameState.Playing);
        public void SetWon()     => Transition(GameState.Won);
        public void SetLost()    => Transition(GameState.Lost);
        public void SetLoading() => Transition(GameState.Loading);
        public void SetPaused()  => Transition(GameState.Paused);

        public bool IsPlaying => _current == GameState.Playing;

        void Transition(GameState next)
        {
            if (_current == next) return;
            var prev = _current;
            _current = next;
            Debug.Log($"[GameState] {prev} → {next}");
            OnStateChanged?.Invoke(prev, next);
        }
    }
}

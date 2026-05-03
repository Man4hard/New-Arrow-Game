// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using System.Collections;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Destroys the arrow GameObject after a countdown once Begin() is called.
    /// The timer can be cancelled (e.g. on collision) via Cancel().
    /// </summary>
    public class ArrowDestroyTimer : MonoBehaviour
    {
        [SerializeField] float _delay = 5f;

        Coroutine _routine;
        bool      _active;

        public event Action OnExpired;

        public void Begin()
        {
            if (_active) return;
            _active  = true;
            _routine = StartCoroutine(Countdown());
        }

        public void Cancel()
        {
            if (!_active) return;
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
            _active  = false;
        }

        IEnumerator Countdown()
        {
            yield return new WaitForSeconds(_delay);
            if (!_active) yield break;

            OnExpired?.Invoke();
            Destroy(gameObject);
        }

        void OnDestroy() => Cancel();
    }
}

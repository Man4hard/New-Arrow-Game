// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Manages colour state for an arrow (LineRenderer + optional SpriteRenderer head).
    /// Stores original colours on first use and can flash a failure colour
    /// before reverting automatically.
    /// </summary>
    public class ArrowMaterial : MonoBehaviour
    {
        [Header("Failure Flash")]
        [SerializeField] Color _failureColor   = Color.red;
        [SerializeField] float _flashDuration  = 0.5f;

        // Typed lists — no Component base-class ambiguity
        readonly List<LineRenderer>   _lineRenderers   = new();
        readonly List<SpriteRenderer> _spriteRenderers = new();

        // Stored original colours
        readonly Dictionary<LineRenderer,   Color> _lineColors   = new();
        readonly Dictionary<SpriteRenderer, Color> _spriteColors = new();

        Coroutine _flashRoutine;

        // ── Registration ──────────────────────────────────────────────

        public void AddRenderer(LineRenderer lr)
        {
            if (lr == null || _lineRenderers.Contains(lr)) return;

            // Instance the material so we don't affect shared assets
            if (lr.sharedMaterial != null)
                lr.material = new Material(lr.sharedMaterial);

            _lineRenderers.Add(lr);
            if (lr.material != null)
                _lineColors[lr] = lr.material.color;
        }

        public void AddRenderer(SpriteRenderer sr)
        {
            if (sr == null || _spriteRenderers.Contains(sr)) return;
            _spriteRenderers.Add(sr);
            _spriteColors[sr] = sr.color;
        }

        // ── Colour control ────────────────────────────────────────────

        public void ShowFailureColor()
        {
            SetAll(_failureColor);

            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        public void ResetColor()
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
                _flashRoutine = null;
            }
            RestoreAll();
        }

        // ── Internals ─────────────────────────────────────────────────

        IEnumerator FlashRoutine()
        {
            yield return new WaitForSeconds(_flashDuration);
            RestoreAll();
            _flashRoutine = null;
        }

        void SetAll(Color c)
        {
            foreach (var lr in _lineRenderers)
                if (lr != null && lr.material != null)
                    lr.material.color = c;

            foreach (var sr in _spriteRenderers)
                if (sr != null) sr.color = c;
        }

        void RestoreAll()
        {
            foreach (var lr in _lineRenderers)
                if (lr != null && _lineColors.TryGetValue(lr, out Color c) && lr.material != null)
                    lr.material.color = c;

            foreach (var sr in _spriteRenderers)
                if (sr != null && _spriteColors.TryGetValue(sr, out Color c))
                    sr.color = c;
        }

        void OnDestroy()
        {
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        }
    }
}

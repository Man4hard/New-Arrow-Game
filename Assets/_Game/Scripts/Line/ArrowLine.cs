// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Root component for an arrow line object.
    /// Owns and coordinates all sub-systems: animation, click, collision, materials.
    /// Must be on the same GameObject as a LineRenderer.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ArrowLine : MonoBehaviour, ArrowsPuzzle.Core.ISelectable
    {
        [Header("Sub-component References")]
        [SerializeField] LineRenderer      _lineRenderer;
        [SerializeField] ArrowAnimator     _animator;
        [SerializeField] ArrowDestroyTimer _destroyTimer;
        [SerializeField] ArrowColliderSync _colliderSync;
        [SerializeField] ArrowHead         _head;
        [SerializeField] SpriteRenderer    _headSprite;
        [SerializeField] ArrowMaterial     _material;

        [Header("Audio")]
        [SerializeField] string _collisionSoundKey = "collision";

        // ── State ─────────────────────────────────────────────────────
        bool _initialized;
        bool _collided;
        bool _lifeLostThisCollision;

        ArrowLineRegistry _registry;

        // ── Public accessors ──────────────────────────────────────────
        public LineRenderer   Renderer  => _lineRenderer;
        public ArrowAnimator  Animator  => _animator;
        public bool           IsReady   => _initialized;

        /// <summary>True while the line may be tapped by the player.</summary>
        public bool IsClickable
        {
            get
            {
                if (_collided) return false;
                if (_animator == null) return true;
                return !_animator.IsRunning;
            }
        }

        // ── Initialisation ────────────────────────────────────────────

        public void Setup(ArrowLineRegistry registry)
        {
            if (_initialized)
            {
                Debug.LogWarning($"[ArrowLine] {name} already set up.", this);
                return;
            }

            if (registry == null)
            {
                Debug.LogError($"[ArrowLine] Registry is null — cannot set up {name}.", this);
                return;
            }

            _registry = registry;

            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null || _lineRenderer.positionCount < 2)
            {
                Debug.LogError($"[ArrowLine] {name}: LineRenderer needs ≥ 2 positions.", this);
                return;
            }

            // Pass references down
            _animator?.Setup(_lineRenderer);
            _colliderSync?.Setup(_lineRenderer);
            _material?.AddRenderer(_lineRenderer);

            if (_head != null)
            {
                _head.gameObject.SetActive(true);
                _head.Setup(_lineRenderer, this);
                _head.OnCollision += HandleHeadCollision;
                if (_headSprite != null)
                    _material?.AddRenderer(_headSprite);
            }

            if (_animator != null)
            {
                _animator.OnPositionsChanged   += _colliderSync.Sync;
                _animator.OnStarted            += HandleAnimationStarted;
                _animator.OnStopped            += HandleAnimationStopped;
                _animator.OnForwardCompleted   += HandleForwardCompleted;
                _animator.OnBackwardCompleted  += HandleBackwardCompleted;
            }

            _initialized = true;
            _registry.Register(this);
        }

        // ── ISelectable ───────────────────────────────────────────────

        public void OnSelected(UnityEngine.Vector3 worldPosition)
        {
            if (!IsClickable) return;

            _destroyTimer?.Begin();
            _animator?.PlayForward();
        }

        // ── Event handlers ────────────────────────────────────────────

        void HandleAnimationStarted(bool forward)
        {
            if (!forward) return;
            _collided              = false;
            _lifeLostThisCollision = false;
            _head?.ResetCollisionState();
            if (_animator != null) _animator.ZOffset = 0f;
        }

        void HandleAnimationStopped()
        {
            if (_animator != null && _animator.IsForward)
            {
                _collided              = false;
                _lifeLostThisCollision = false;
            }
            _head?.ResetCollisionState();
        }

        void HandleForwardCompleted()
        {
            if (_collided)
            {
                // Collision happened but line somehow completed — clean up
                ForceReset();
                return;
            }
            // Successful clear — remove from registry (win condition check)
            _registry?.Unregister(this);
        }

        void HandleBackwardCompleted()
        {
            // Line has returned to start after collision — allow re-tap
            ForceReset();
        }

        void HandleHeadCollision(Collider2D other)
        {
            if (_collided) return;
            StartReverse();
        }

        // ── Collision logic ───────────────────────────────────────────

        void StartReverse()
        {
            if (_collided) return;
            _collided = true;

            if (Core.SoundManager.Exists)
                Core.SoundManager.Instance.Play(_collisionSoundKey);

            _animator?.Stop();
            if (_animator != null) _animator.ZOffset = -1f;
            _animator?.PlayBackward();

            _destroyTimer?.Cancel();
            _material?.ShowFailureColor();

            if (!_lifeLostThisCollision && Core.LifeTracker.Exists)
            {
                _lifeLostThisCollision = true;
                Core.LifeTracker.Instance.LoseOne();
            }
        }

        void ForceReset()
        {
            _collided              = false;
            _lifeLostThisCollision = false;
            if (_animator != null) _animator.ZOffset = 0f;
            _material?.ResetColor();
            _head?.ResetCollisionState();
        }

        // ── Cleanup ───────────────────────────────────────────────────

        public void Cleanup()
        {
            if (!_initialized) return;

            if (_animator != null)
            {
                _animator.OnPositionsChanged  -= _colliderSync.Sync;
                _animator.OnStarted           -= HandleAnimationStarted;
                _animator.OnStopped           -= HandleAnimationStopped;
                _animator.OnForwardCompleted  -= HandleForwardCompleted;
                _animator.OnBackwardCompleted -= HandleBackwardCompleted;
                _animator.Stop();
                _animator.ZOffset = 0f;
            }

            _material?.ResetColor();
            _destroyTimer?.Cancel();
            _initialized = false;
        }

        void OnDestroy()
        {
            if (_head != null) _head.OnCollision -= HandleHeadCollision;
            _registry?.Unregister(this);
            Cleanup();
        }
    }
}

// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Follows the tip of the LineRenderer every LateUpdate and rotates
    /// to face the direction of travel. Hosts the CircleCollider2D that
    /// detects when this arrow hits another arrow's body.
    ///
    /// Attaches physics components at runtime only if they are absent,
    /// so the prefab can pre-configure them instead.
    /// </summary>
    [ExecuteInEditMode]
    public class ArrowHead : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] float _rotationOffset = 0f;

        [Header("Collider")]
        [SerializeField] float _colliderRadius = 0.3f;

        LineRenderer       _lr;
        ArrowLine          _ownerLine;
        CircleCollider2D   _col;
        bool               _collided;
        bool               _initialized;

        public event Action<Collider2D> OnCollision;

        // ── Setup ─────────────────────────────────────────────────────

        public void Setup(LineRenderer lr, ArrowLine owner)
        {
            if (_initialized) return;

            _lr        = lr;
            _ownerLine = owner;
            _collided  = false;

            EnsurePhysics();
            _initialized = true;
            enabled      = true;
        }

        public void ResetCollisionState() => _collided = false;

        // ── Position tracking ─────────────────────────────────────────

        void LateUpdate()
        {
            if (_lr == null || _lr.positionCount < 2) return;

            int last = _lr.positionCount - 1;
            Vector3 tip  = _lr.GetPosition(last);
            Vector3 prev = _lr.GetPosition(last - 1);

            transform.localPosition = tip;

            Vector3 dir = (tip - prev).normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + _rotationOffset);
        }

        // ── Collision ─────────────────────────────────────────────────

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!_initialized || _ownerLine == null || _collided) return;

            // Ignore self-collisions
            ArrowLine otherLine = other.GetComponent<ArrowLine>()
                               ?? other.GetComponentInParent<ArrowLine>();

            if (otherLine == null || otherLine == _ownerLine) return;

            _collided = true;
            OnCollision?.Invoke(other);
        }

        // ── Cleanup ───────────────────────────────────────────────────

        void OnDestroy()
        {
            // Unsubscribing handled by ArrowLine via the event
        }

        // ── Private helpers ───────────────────────────────────────────

        void EnsurePhysics()
        {
            // Rigidbody2D — kinematic, no gravity
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb              = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType     = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0f;
            }

            // CircleCollider2D — trigger only
            _col = GetComponent<CircleCollider2D>();
            if (_col == null)
            {
                _col        = gameObject.AddComponent<CircleCollider2D>();
                _col.radius = _colliderRadius;
            }
            _col.isTrigger = true;
        }
    }
}

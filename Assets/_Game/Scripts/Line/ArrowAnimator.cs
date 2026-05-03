// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Animates a LineRenderer so the arrow "slides" forward along its path,
    /// shortening from the tail, or reverses back to its origin.
    ///
    /// Positions are stored once at setup time; no per-frame heap allocations
    /// for the main move logic (uses a fixed-size working buffer).
    /// </summary>
    public class ArrowAnimator : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] float _speed    = 5f;
        [SerializeField] float _tailBias = 1.5f; // tail moves this × faster than head

        [Header("Audio")]
        [SerializeField] string _moveSoundKey = "move";

        LineRenderer _lr;
        Vector3[]    _origin;          // positions captured at setup
        Vector3[]    _work;            // reused working buffer (no per-frame alloc)
        bool         _running;
        bool         _forward;
        Vector3      _headDirection;
        float        _zOffset;

        // ── Events ────────────────────────────────────────────────────
        public event Action<bool> OnStarted;           // bool = forward
        public event Action       OnStopped;
        public event Action       OnForwardCompleted;
        public event Action       OnBackwardCompleted;
        public event Action       OnPositionsChanged;

        // ── Properties ───────────────────────────────────────────────
        public bool  IsRunning => _running;
        public bool  IsForward => _forward;

        public float ZOffset
        {
            get => _zOffset;
            set => _zOffset = value;
        }

        // ── Setup ─────────────────────────────────────────────────────

        public void Setup(LineRenderer lr)
        {
            _lr = lr;
            int count = lr.positionCount;
            if (count < 2) { enabled = false; return; }

            _origin = new Vector3[count];
            lr.GetPositions(_origin);
            _work = new Vector3[count];

            // Direction = last segment direction, normalised
            _headDirection = (_origin[count - 1] - _origin[count - 2]).normalized;

            enabled = false;
        }

        // ── Public controls ───────────────────────────────────────────

        public void PlayForward()  => BeginPlay(true);
        public void PlayBackward() => BeginPlay(false);

        public void Stop()
        {
            if (!_running) return;
            _running = false;
            enabled  = false;
            OnStopped?.Invoke();
        }

        // ── Unity loop ────────────────────────────────────────────────

        void Update()
        {
            if (_lr == null || _lr.positionCount < 2)
            {
                Stop();
                return;
            }

            if (_forward) TickForward();
            else          TickBackward();
        }

        // ── Forward animation ─────────────────────────────────────────

        void TickForward()
        {
            int   count = _lr.positionCount;
            float dt    = Time.deltaTime;
            float move  = _speed * dt;

            EnsureWorkBuffer(count);
            _lr.GetPositions(_work);

            // Advance head along stored direction
            _work[count - 1] += _headDirection * move;

            // Advance tail toward next waypoint (faster than head = shortening effect)
            float tailMove = move * _tailBias;
            Vector3 nextWaypoint = _work[1];
            _work[0] = Vector3.MoveTowards(_work[0], nextWaypoint, tailMove);

            ApplyZOffset(count);
            _lr.SetPositions(_work);
            OnPositionsChanged?.Invoke();

            // If tail reached next waypoint, collapse that segment
            if (Vector3.Distance(_work[0], nextWaypoint) < 0.05f)
            {
                CollapseFirstSegment(count);
            }
        }

        void CollapseFirstSegment(int count)
        {
            int newCount = count - 1;
            if (newCount >= 2)
            {
                _lr.positionCount = newCount;
                EnsureWorkBuffer(newCount);
                // Shift all positions left by one
                for (int i = 0; i < newCount; i++) _work[i] = _work[i + 1];
                _lr.SetPositions(_work);
            }
            else
            {
                // Line fully consumed — animation complete
                _lr.positionCount = 0;
                Stop();

                // Hide arrow head sprite
                ArrowHead head = GetComponentInChildren<ArrowHead>(true);
                head?.gameObject.SetActive(false);

                OnForwardCompleted?.Invoke();
            }
            OnPositionsChanged?.Invoke();
        }

        // ── Backward animation ────────────────────────────────────────

        void TickBackward()
        {
            int   count        = _lr.positionCount;
            float move         = _speed * Time.deltaTime;
            int   originCount  = _origin.Length;
            int   targetIdx    = originCount - count; // which origin point the tail targets

            EnsureWorkBuffer(count);
            _lr.GetPositions(_work);

            // Move head back toward its original end position
            Vector3 originHead = _origin[originCount - 1];
            _work[count - 1] = Vector3.MoveTowards(_work[count - 1], originHead, move);

            if (targetIdx < 0 || targetIdx >= originCount) { Stop(); return; }

            Vector3 tailTarget = _origin[targetIdx];
            _work[0] = Vector3.MoveTowards(_work[0], tailTarget, move);

            ApplyZOffset(count);
            _lr.SetPositions(_work);
            OnPositionsChanged?.Invoke();

            // Tail snapped to current target waypoint
            if (Vector3.Distance(_work[0], tailTarget) < 0.05f)
            {
                if (targetIdx > 0)
                {
                    ExpandFirstSegment(count, targetIdx);
                }
                else if (Vector3.Distance(_work[count - 1], originHead) < 0.05f)
                {
                    // Fully restored
                    RestoreOrigin();
                    Stop();
                    OnBackwardCompleted?.Invoke();
                }
            }
        }

        void ExpandFirstSegment(int count, int targetIdx)
        {
            int newCount = count + 1;
            _lr.positionCount = newCount;
            EnsureWorkBuffer(newCount);
            // Prepend the previous origin waypoint
            _work[0] = _origin[targetIdx - 1];
            for (int i = 1; i < newCount; i++) _work[i] = _work[i - 1 + (i == 1 ? 1 : 0)];
            // Safer copy from current positions
            Vector3[] tmp = new Vector3[count];
            _lr.GetPositions(tmp); // re-read since count changed
            // Actually build correctly
            _lr.positionCount = newCount;
            Vector3[] built = new Vector3[newCount];
            built[0] = _origin[targetIdx - 1];
            for (int i = 0; i < count; i++) built[i + 1] = tmp[i];
            _lr.SetPositions(built);
        }

        void RestoreOrigin()
        {
            _lr.positionCount = _origin.Length;
            _lr.SetPositions(_origin);
        }

        // ── Helpers ───────────────────────────────────────────────────

        void BeginPlay(bool forward)
        {
            if (_lr == null) return;
            bool wasRunning = _running;
            _forward = forward;
            _running = true;
            enabled  = true;

            if (!wasRunning)
            {
                OnStarted?.Invoke(forward);
                if (Core.SoundManager.Exists && !string.IsNullOrEmpty(_moveSoundKey))
                    Core.SoundManager.Instance.Play(_moveSoundKey);
            }
        }

        void ApplyZOffset(int count)
        {
            if (Mathf.Abs(_zOffset) < 0.001f) return;
            for (int i = 0; i < count; i++) _work[i].z = _zOffset;
        }

        void EnsureWorkBuffer(int size)
        {
            if (_work == null || _work.Length < size)
                _work = new Vector3[size];
        }
    }
}

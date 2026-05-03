// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Keeps a pool of BoxCollider2D segments that mirror the LineRenderer path.
    /// Called each time the animator changes positions so the physics world
    /// stays in sync with the visual line.
    /// Segments are pooled (hidden/shown) rather than destroyed to avoid GC spikes.
    /// </summary>
    public class ArrowColliderSync : MonoBehaviour
    {
        [Header("Segment Prefab")]
        [SerializeField] GameObject _segmentPrefab;

        [Header("Collider Sizing")]
        [SerializeField] float _thickness  = 0.2f;
        [SerializeField] float _lengthPad  = 0.1f;

        LineRenderer        _lr;
        List<GameObject>    _pool = new();

        // ── Setup ─────────────────────────────────────────────────────

        public void Setup(LineRenderer lr)
        {
            _lr = lr;
            RebuildAll();
        }

        // ── Called by ArrowAnimator.OnPositionsChanged ────────────────

        public void Sync()
        {
            if (_lr == null || _segmentPrefab == null) return;

            int count   = _lr.positionCount;
            int needed  = Mathf.Max(0, count - 1);
            bool useWorld = _lr.useWorldSpace;

            // Show/hide to match needed count
            EnsurePoolSize(needed);
            for (int i = 0; i < _pool.Count; i++)
                _pool[i].SetActive(i < needed);

            for (int i = 0; i < needed; i++)
            {
                Vector3 a = _lr.GetPosition(i);
                Vector3 b = _lr.GetPosition(i + 1);

                if (!useWorld)
                {
                    a = _lr.transform.TransformPoint(a);
                    b = _lr.transform.TransformPoint(b);
                }

                Vector3 mid = (a + b) * 0.5f;
                Vector3 dir = b - a;
                float   len = dir.magnitude;
                float   ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                GameObject seg = _pool[i];
                seg.transform.position = mid;
                seg.transform.rotation = Quaternion.Euler(0f, 0f, ang);

                BoxCollider2D box = seg.GetComponent<BoxCollider2D>();
                if (box != null)
                    box.size = new Vector2(len + _lengthPad, _thickness);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────

        void RebuildAll()
        {
            // Hide all existing
            foreach (var seg in _pool) seg.SetActive(false);
            Sync();
        }

        void EnsurePoolSize(int needed)
        {
            while (_pool.Count < needed)
            {
                var seg = Instantiate(_segmentPrefab, transform);
                seg.SetActive(false);
                _pool.Add(seg);
            }
        }

        void OnDestroy()
        {
            foreach (var seg in _pool)
                if (seg != null) Destroy(seg);
            _pool.Clear();
        }
    }
}

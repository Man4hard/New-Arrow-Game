// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Game
{
    /// <summary>
    /// Snaps all LineRenderer positions to a grid in the Editor (or optionally at runtime).
    /// Useful for keeping level data on clean integer/half-unit coordinates.
    /// Enable <see cref="SnapInEditor"/> to snap during scene editing,
    /// Enable <see cref="SnapAtRuntime"/> to snap during Play mode (off by default).
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class GridSnapper : MonoBehaviour
    {
        [SerializeField] float _cellSize      = 1f;
        [SerializeField] bool  _snapInEditor  = true;
        [SerializeField] bool  _snapAtRuntime = false;

        LineRenderer _lr;

        void Awake() => _lr = GetComponent<LineRenderer>();

        void LateUpdate()
        {
            if (_lr == null) return;

            bool shouldRun = Application.isPlaying ? _snapAtRuntime : _snapInEditor;
            if (!shouldRun) return;

            SnapAll();
        }

        void OnValidate() => _lr = GetComponent<LineRenderer>();

        void SnapAll()
        {
            int n = _lr.positionCount;
            for (int i = 0; i < n; i++)
            {
                Vector3 p = _lr.GetPosition(i);
                p.x = Snap(p.x);
                p.y = Snap(p.y);
                p.z = 0f;
                _lr.SetPosition(i, p);
            }
        }

        float Snap(float v) => Mathf.Round(v / _cellSize) * _cellSize;

        [ContextMenu("Snap Now")]
        void SnapNow() { _lr = GetComponent<LineRenderer>(); SnapAll(); }
    }
}

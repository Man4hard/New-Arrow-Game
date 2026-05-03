// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Reads touch/mouse input each frame and writes it into PlayerInputData.
    /// Supports both mobile touch and editor mouse seamlessly.
    /// Call LockInput / UnlockInput to suppress all input (e.g. during transitions).
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class InputReader : MonoSingleton<InputReader>
    {
        [SerializeField] PlayerInputData _inputData;

        bool _locked;

        public bool IsLocked => _locked;

        public void Lock()   => _locked = true;
        public void Unlock() => _locked = false;

        void Update()
        {
            _inputData.ResetFrame();
            if (_locked) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            ReadMouse();
#else
            if (Input.touchCount > 0)
                ReadTouch();
            else
                ReadMouse();
#endif
        }

        void ReadTouch()
        {
            Touch t = Input.GetTouch(0);
            switch (t.phase)
            {
                case TouchPhase.Began:
                    _inputData.RecordPress(t.position);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    _inputData.RecordHold(t.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _inputData.RecordRelease(t.position);
                    break;
            }
        }

        void ReadMouse()
        {
            Vector3 pos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
                _inputData.RecordPress(pos);
            else if (Input.GetMouseButton(0))
                _inputData.RecordHold(pos);
            else if (Input.GetMouseButtonUp(0))
                _inputData.RecordRelease(pos);
        }
    }
}

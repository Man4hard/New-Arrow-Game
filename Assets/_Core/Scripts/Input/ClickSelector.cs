// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;
using UnityEngine.EventSystems;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// On every tap/press, casts a Physics2D overlap at the world position and
    /// notifies the first ISelectable found. Works with a configurable radius
    /// so touches feel forgiving on small mobile screens.
    /// </summary>
    [DefaultExecutionOrder(-90)]
    public class ClickSelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] PlayerInputData _inputData;

        [Header("Raycast Settings")]
        [SerializeField] LayerMask _selectableLayers;
        [SerializeField][Range(0.05f, 2f)] float _touchRadius = 0.4f;

        Camera _cam;

        void Awake()
        {
            _cam = Camera.main;
            if (_cam == null)
                Debug.LogError("[ClickSelector] No Main Camera found. Tag your camera as MainCamera.");
        }

        void Update()
        {
            if (_inputData == null || !_inputData.PressedThisFrame) return;
            if (IsPointerOverUI()) return;

            Vector3 worldPos = ScreenToWorld(_inputData.ScreenPosition);
            TrySelect(worldPos);
        }

        void TrySelect(Vector3 worldPos)
        {
            Collider2D hit = Physics2D.OverlapCircle(worldPos, _touchRadius, _selectableLayers);
            if (hit == null) return;

            ISelectable selectable = hit.GetComponent<ISelectable>()
                                  ?? hit.GetComponentInParent<ISelectable>();

            selectable?.OnSelected(worldPos);
        }

        Vector3 ScreenToWorld(Vector3 screenPos)
        {
            Vector3 w = _cam.ScreenToWorldPoint(screenPos);
            w.z = 0f;
            return w;
        }

        bool IsPointerOverUI()
            => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}

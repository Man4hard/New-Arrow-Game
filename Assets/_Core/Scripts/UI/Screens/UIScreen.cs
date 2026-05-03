// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Base class for all UI panels/screens.
    /// Hides/shows the panel's root CanvasGroup so transitions are possible.
    /// Override OnShow / OnHide to add animations.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        CanvasGroup _group;

        protected virtual void Awake()
        {
            _group = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            _group.alpha          = 1f;
            _group.interactable   = true;
            _group.blocksRaycasts = true;
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            OnHide();
            _group.alpha          = 0f;
            _group.interactable   = false;
            _group.blocksRaycasts = false;
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}

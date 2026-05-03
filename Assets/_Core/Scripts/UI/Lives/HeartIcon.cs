// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;
using UnityEngine.UI;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Controls a single heart icon in the HUD.
    /// Swaps the sprite between full and empty states.
    /// </summary>
    public class HeartIcon : MonoBehaviour
    {
        [SerializeField] Image  _image;
        [SerializeField] Sprite _fullSprite;
        [SerializeField] Sprite _emptySprite;

        void Awake()
        {
            if (_image == null)
                _image = GetComponent<Image>();
        }

        public void SetFull()
        {
            if (_image == null) return;
            _image.sprite = _fullSprite;
            _image.enabled = true;
        }

        public void SetEmpty()
        {
            if (_image == null) return;
            _image.sprite = _emptySprite != null ? _emptySprite : _fullSprite;
            _image.enabled = _emptySprite != null; // hide if no empty sprite supplied
        }

        public void SetState(bool full)
        {
            if (full) SetFull(); else SetEmpty();
        }
    }
}

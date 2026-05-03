// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Implement this on any component that should respond to tap/click input.
    /// </summary>
    public interface ISelectable
    {
        void OnSelected(Vector3 worldPosition);
    }
}

// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// ScriptableObject that carries raw pointer state between InputReader
    /// and any system that needs to query it. Decouples producer from consumers.
    /// Create via: Assets → Create → ArrowsPuzzle → PlayerInputData
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerInputData", menuName = "ArrowsPuzzle/PlayerInputData")]
    public class PlayerInputData : ScriptableObject
    {
        public bool PressedThisFrame  { get; private set; }
        public bool HeldThisFrame     { get; private set; }
        public bool ReleasedThisFrame { get; private set; }
        public Vector3 ScreenPosition { get; private set; }

        public void ResetFrame()
        {
            PressedThisFrame  = false;
            HeldThisFrame     = false;
            ReleasedThisFrame = false;
        }

        public void RecordPress(Vector3 screenPos)
        {
            ScreenPosition   = screenPos;
            PressedThisFrame = true;
            HeldThisFrame    = false;
        }

        public void RecordHold(Vector3 screenPos)
        {
            ScreenPosition  = screenPos;
            HeldThisFrame   = true;
        }

        public void RecordRelease(Vector3 screenPos)
        {
            ScreenPosition    = screenPos;
            ReleasedThisFrame = true;
            HeldThisFrame     = false;
        }
    }
}

// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    [System.Serializable]
    public class SoundEntry
    {
        [Tooltip("Key used to play this sound via SoundManager.Play(key)")]
        public string Key;

        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume = 1f;

        [Range(0.5f, 2f)]
        public float Pitch = 1f;

        public bool Loop = false;
        public bool Spatial = false;

        [Tooltip("Only used when Spatial is true")]
        public float MinDistance = 1f;

        [Tooltip("Only used when Spatial is true")]
        public float MaxDistance = 20f;
    }
}

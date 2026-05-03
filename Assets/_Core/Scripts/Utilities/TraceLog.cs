// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Thin wrapper around UnityEngine.Debug that can be globally silenced
    /// in release builds by setting <see cref="Enabled"/> to false.
    /// </summary>
    public static class TraceLog
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public static bool Enabled = true;
#else
        public static bool Enabled = false;
#endif

        public static void Info(string msg, Object ctx = null)
        {
            if (!Enabled) return;
            Debug.Log($"[INFO] {msg}", ctx);
        }

        public static void Warn(string msg, Object ctx = null)
        {
            if (!Enabled) return;
            Debug.LogWarning($"[WARN] {msg}", ctx);
        }

        public static void Error(string msg, Object ctx = null)
        {
            // Errors always log regardless of Enabled flag
            Debug.LogError($"[ERROR] {msg}", ctx);
        }
    }
}

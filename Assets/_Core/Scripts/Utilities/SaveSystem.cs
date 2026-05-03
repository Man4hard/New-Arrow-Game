// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Thin wrapper around PlayerPrefs with a debounce guard to avoid
    /// hammering the disk on every setter call.
    /// </summary>
    public static class SaveSystem
    {
        const float MinSaveInterval = 0.3f;
        static float _lastSavedAt = -999f;

        public static int GetInt(string key, int defaultValue = 0)
            => PlayerPrefs.GetInt(key, defaultValue);

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            FlushDebounced();
        }

        public static float GetFloat(string key, float defaultValue = 0f)
            => PlayerPrefs.GetFloat(key, defaultValue);

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            FlushDebounced();
        }

        public static string GetString(string key, string defaultValue = "")
            => PlayerPrefs.GetString(key, defaultValue);

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            FlushDebounced();
        }

        public static void FlushImmediate()
        {
            PlayerPrefs.Save();
            _lastSavedAt = Time.unscaledTime;
        }

        static void FlushDebounced()
        {
            if (Time.unscaledTime - _lastSavedAt >= MinSaveInterval)
                FlushImmediate();
        }
    }

    /// <summary>
    /// Centralised key constants to avoid magic strings scattered through the codebase.
    /// </summary>
    public static class PrefKeys
    {
        public const string LevelProgress = "level_progress";
        public const string SoundEnabled   = "sound_enabled";
        public const string HapticEnabled  = "haptic_enabled";
    }
}

// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Central audio manager. Register sound entries in the Inspector,
    /// then play them anywhere via SoundManager.Instance.Play("key").
    /// Uses a small pool of AudioSources to avoid GC pressure.
    /// </summary>
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [Header("Sound Library")]
        [SerializeField] List<SoundEntry> _sounds = new();

        [Header("Pool")]
        [SerializeField][Range(4, 32)] int _poolSize = 8;

        [Header("Music")]
        [SerializeField] AudioSource _musicSource;

        Dictionary<string, SoundEntry> _lookup;
        Queue<AudioSource> _sourcePool;
        bool _sfxEnabled  = true;
        bool _musicEnabled = true;

        protected override void Awake()
        {
            base.Awake();
            BuildLookup();
            BuildPool();
            EnsureMusicSource();

            _sfxEnabled   = SaveSystem.GetInt(PrefKeys.SoundEnabled, 1) == 1;
            _musicEnabled = _sfxEnabled;
        }

        // ── Public API ──────────────────────────────────────────────

        public void Play(string key)
        {
            if (!_sfxEnabled) return;
            if (!_lookup.TryGetValue(key, out var entry)) { Warn(key); return; }

            AudioSource src = RentSource();
            if (src == null) return;

            ConfigureSource(src, entry, false, Vector3.zero);
            src.Play();

            if (!entry.Loop)
                StartCoroutine(ReturnWhenDone(src));
        }

        public void PlayAt(string key, Vector3 worldPos)
        {
            if (!_sfxEnabled) return;
            if (!_lookup.TryGetValue(key, out var entry)) { Warn(key); return; }

            AudioSource src = RentSource();
            if (src == null) return;

            src.transform.position = worldPos;
            ConfigureSource(src, entry, true, worldPos);
            src.Play();

            if (!entry.Loop)
                StartCoroutine(ReturnWhenDone(src));
        }

        public void PlayMusic(AudioClip clip)
        {
            if (_musicSource == null || clip == null) return;
            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.volume = _musicEnabled ? 1f : 0f;
            _musicSource.Play();
        }

        public void StopMusic()
            => _musicSource?.Stop();

        public void SetSfxEnabled(bool on)
        {
            _sfxEnabled = on;
            SaveSystem.SetInt(PrefKeys.SoundEnabled, on ? 1 : 0);
            if (!on) StopMusic();
        }

        public bool IsSfxEnabled => _sfxEnabled;

        // ── Internals ────────────────────────────────────────────────

        void BuildLookup()
        {
            _lookup = new Dictionary<string, SoundEntry>(StringComparer.Ordinal);
            foreach (var e in _sounds)
                if (!string.IsNullOrEmpty(e.Key) && e.Clip != null)
                    _lookup[e.Key] = e;
        }

        void BuildPool()
        {
            _sourcePool = new Queue<AudioSource>(_poolSize);
            for (int i = 0; i < _poolSize; i++)
            {
                var go = new GameObject($"SFX_Source_{i}");
                go.transform.SetParent(transform);
                go.SetActive(false);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                _sourcePool.Enqueue(src);
            }
        }

        void EnsureMusicSource()
        {
            if (_musicSource != null) return;
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop        = true;
            _musicSource.spatialBlend = 0f;
        }

        AudioSource RentSource()
        {
            if (_sourcePool.Count == 0) return null;
            var src = _sourcePool.Dequeue();
            src.gameObject.SetActive(true);
            return src;
        }

        void ReturnSource(AudioSource src)
        {
            if (src == null) return;
            src.Stop();
            src.clip = null;
            src.gameObject.SetActive(false);
            _sourcePool.Enqueue(src);
        }

        void ConfigureSource(AudioSource src, SoundEntry e, bool spatial, Vector3 pos)
        {
            src.clip         = e.Clip;
            src.volume       = e.Volume;
            src.pitch        = e.Pitch;
            src.loop         = e.Loop;
            src.spatialBlend = (spatial || e.Spatial) ? 1f : 0f;
            src.minDistance  = e.MinDistance;
            src.maxDistance  = e.MaxDistance;
            if (spatial) src.transform.position = pos;
        }

        IEnumerator ReturnWhenDone(AudioSource src)
        {
            yield return new WaitWhile(() => src != null && src.isPlaying);
            ReturnSource(src);
        }

        void Warn(string key) => Debug.LogWarning($"[SoundManager] Key not found: '{key}'");
    }
}

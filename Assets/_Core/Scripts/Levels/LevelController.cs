// Copyright (c) 2025 [YOUR NAME HERE]. MIT License — see LICENSE.txt

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle.Core
{
    /// <summary>
    /// Manages level loading, progression, and retry logic.
    /// Levels are loaded from Resources/Levels sorted by natural name order.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public class LevelController : MonoSingleton<LevelController>
    {
        [Header("Configuration")]
        [SerializeField] bool _randomAfterLastLevel = true;

        [Header("Runtime (read-only)")]
        [SerializeField] int _debugCurrentLevel = 1;

        LevelDefinition[] _allLevels;
        LevelDefinition   _activeLevelInstance;

        public int CurrentLevelNumber
        {
            get => SaveSystem.GetInt(PrefKeys.LevelProgress, 1);
            private set => SaveSystem.SetInt(PrefKeys.LevelProgress, value);
        }

        public LevelDefinition ActiveLevel => _activeLevelInstance;
        public int TotalLevels => _allLevels?.Length ?? 0;

        public event Action OnLevelStarted;
        public event Action OnLevelCompleted;

        protected override void Awake()
        {
            base.Awake();
            LoadAllLevelsFromResources();
        }

        void Start()
        {
            SpawnCurrentLevel();
        }

        // ── Public API ──────────────────────────────────────────────

        public void StartCurrentLevel()
        {
            SpawnCurrentLevel();
        }

        public void RetryCurrentLevel()
        {
            DestroyActiveLevel();
            SpawnCurrentLevel();
        }

        public void AdvanceToNextLevel()
        {
            DestroyActiveLevel();
            CurrentLevelNumber++;
            SpawnCurrentLevel();
        }

        // ── Internals ────────────────────────────────────────────────

        void LoadAllLevelsFromResources()
        {
            _allLevels = Resources.LoadAll<LevelDefinition>("Levels");
            if (_allLevels == null || _allLevels.Length == 0)
            {
                Debug.LogError("[LevelController] No LevelDefinition prefabs found under Resources/Levels.");
                return;
            }

            // Natural sort: "Level 2" before "Level 10"
            Array.Sort(_allLevels, NaturalNameCompare);
            Debug.Log($"[LevelController] Loaded {_allLevels.Length} levels.");
        }

        void SpawnCurrentLevel()
        {
            if (_allLevels == null || _allLevels.Length == 0) return;

            int index = ResolveIndex(CurrentLevelNumber);
            _debugCurrentLevel = CurrentLevelNumber;

            LevelDefinition prefab = _allLevels[index];
            _activeLevelInstance = Instantiate(prefab);
            _activeLevelInstance.Activate();

            GameStateController.Instance?.SetPlaying();
            OnLevelStarted?.Invoke();
        }

        void DestroyActiveLevel()
        {
            if (_activeLevelInstance != null)
            {
                Destroy(_activeLevelInstance.gameObject);
                _activeLevelInstance = null;
            }
        }

        int ResolveIndex(int levelNumber)
        {
            int total = _allLevels.Length;
            if (levelNumber <= total)
                return Mathf.Clamp(levelNumber - 1, 0, total - 1);

            // Past the last level
            return _randomAfterLastLevel
                ? UnityEngine.Random.Range(0, total)
                : (levelNumber - 1) % total;
        }

        static int NaturalNameCompare(LevelDefinition a, LevelDefinition b)
        {
            // Split strings into text and number chunks for proper ordering
            return NaturalSort(a.name, b.name);
        }

        static int NaturalSort(string x, string y)
        {
            int ix = 0, iy = 0;
            while (ix < x.Length && iy < y.Length)
            {
                bool xDigit = char.IsDigit(x[ix]);
                bool yDigit = char.IsDigit(y[iy]);

                if (xDigit && yDigit)
                {
                    int nx = 0, ny = 0;
                    while (ix < x.Length && char.IsDigit(x[ix])) nx = nx * 10 + (x[ix++] - '0');
                    while (iy < y.Length && char.IsDigit(y[iy])) ny = ny * 10 + (y[iy++] - '0');
                    if (nx != ny) return nx.CompareTo(ny);
                }
                else
                {
                    int cmp = char.ToLowerInvariant(x[ix]).CompareTo(char.ToLowerInvariant(y[iy]));
                    if (cmp != 0) return cmp;
                    ix++; iy++;
                }
            }
            return x.Length.CompareTo(y.Length);
        }
    }
}

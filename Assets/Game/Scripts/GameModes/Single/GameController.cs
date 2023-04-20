using System;
using System.Collections;
using System.Collections.Generic;
using Game.Configs;
using Game.Entities;
using Game.World;
using UnityEngine;

namespace Game.GameModes.Single
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private GameConfig _gameConfig;

        // jTODO maybe move to separate scene
        [SerializeField]
        private GameObject EditorOnlyStuff;

        [SerializeField]
        private PlaneController _planePrefab;

        [SerializeField]
        private CameraController _cameraController;

        [SerializeField]
        private Gradient _healthBarGradient;


        public event Action<float> OnTimeChanged;
        public event Action<int> OnScoreChanged;
        public event Action<float, Color> OnHealthChanged;
        public event Action OnGameFailure;
        public event Action OnGameSuccess;
        public event Action<string> OnShowLevelMessage;
        public event Action OnHideLevelMessage;

        // Game Vars
        public int Score => _score;

        private PlaneController _planeController;

        // Level Vars
        private LevelConfig _levelConfig;
        private int _currentLevelIndex;
        private float _currentLevelHealth;
        private float _timePassed;
        private int _numSecondsPassed;
        private int _score;
        private bool _isLevelRunning;

        // Spawn Vars
        private System.Random _random = new System.Random();
        private List<WorldLocation> _currentWorldLocations;
        private readonly List<int> _currentWorldLocationIndexes = new List<int>();
        private Transform _necessitiesParent;
        private Transform _targetsParent;
        private Coroutine _spawnNecessitiesCoroutine;
        private Coroutine _spawnTargetsCoroutine;


        private void Awake()
        {
            // Cleanup
            Destroy(EditorOnlyStuff);
        }

        private void Start()
        {
            _isLevelRunning = false;

            _planeController = Instantiate(_planePrefab, Vector3.back * 110, Quaternion.identity);

            _cameraController.Init(_gameConfig, _planeController);
        }

        private void Update()
        {
            if (!_isLevelRunning)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                _isLevelRunning = false;
                OnLevelSuccess();
                return;
            }

            _timePassed += Time.deltaTime;

            if (_timePassed >= _numSecondsPassed + 1)
            {
                _numSecondsPassed += 1;

                var timeLeft = _levelConfig.Duration - _numSecondsPassed;
                OnTimeChanged?.Invoke(timeLeft);

                if (timeLeft <= 0)
                {
                    _isLevelRunning = false;
                    // jTODO maybe refactor to Action
                    OnLevelSuccess();
                    return;
                }
            }

            _currentLevelHealth -= _levelConfig.DecayRate * Time.deltaTime;

            var healthRatio = Mathf.Clamp01(_currentLevelHealth / _levelConfig.TotalHealth);
            var color = _healthBarGradient.Evaluate(healthRatio);
            OnHealthChanged?.Invoke(healthRatio, color);

            if (_currentLevelHealth <= 0)
            {
                _isLevelRunning = false;
                OnGameFailure?.Invoke();
            }
        }

        // Called from GameEvent
        public void StartGame()
        {
            _score = 0;
            OnScoreChanged?.Invoke(_score);

            SwitchToLevel(0);
        }

        // Called from GameEvent
        public void HelpReceived()
        {
            _score += 100;
            OnScoreChanged?.Invoke(_score);

            // Get 10% of health back
            _currentLevelHealth += 0.1f * _levelConfig.TotalHealth;

            // jTODO play sfx
        }

        private void SwitchToLevel(int levelIndex)
        {
            if (levelIndex >= _gameConfig.LevelConfigs.Length)
            {
                // Show Game Success
                OnGameSuccess?.Invoke();
                return;
            }

            CleanUp();

            _currentLevelIndex = levelIndex;

            if (_gameConfig.LevelConfigs[_currentLevelIndex] == null)
            {
                throw new Exception($"Undefined level {_currentLevelIndex}");
            }

            InitLevel();
        }

        private void CleanUp()
        {
            if (_necessitiesParent != null)
                Destroy(_necessitiesParent.gameObject);
            if (_targetsParent != null)
                Destroy(_targetsParent.gameObject);

            if (_spawnNecessitiesCoroutine != null)
            {
                StopCoroutine(_spawnNecessitiesCoroutine);
                _spawnNecessitiesCoroutine = null;
            }

            if (_spawnTargetsCoroutine != null)
            {
                StopCoroutine(_spawnTargetsCoroutine);
                _spawnTargetsCoroutine = null;
            }
        }

        private void InitLevel()
        {
            _levelConfig = _gameConfig.LevelConfigs[_currentLevelIndex];

            StartCoroutine(InitLevelCo());
        }

        private IEnumerator InitLevelCo()
        {
            OnTimeChanged?.Invoke(_levelConfig.Duration);
            OnHealthChanged?.Invoke(1, _healthBarGradient.Evaluate(1));

            OnShowLevelMessage?.Invoke(_levelConfig.Title);
            yield return new WaitForSeconds(3f);
            OnHideLevelMessage?.Invoke();

            _currentLevelHealth = _levelConfig.TotalHealth;

            _timePassed = 0f;
            _numSecondsPassed = 0;


            _necessitiesParent = new GameObject("Necessities").transform;
            _targetsParent = new GameObject("Targets").transform;
            _spawnNecessitiesCoroutine = StartCoroutine(SpawnNecessities());
            _spawnTargetsCoroutine = StartCoroutine(SpawnTargets());

            _isLevelRunning = true;
        }

        private IEnumerator SpawnNecessities()
        {
            while (true)
            {
                var range = UnityEngine.Random.Range(_levelConfig.NecessitySpawnMinDelay, _levelConfig.NecessitySpawnMaxDelay);
                yield return new WaitForSeconds(range);

                SpawnNextNecessity();
            }
        }

        private IEnumerator SpawnTargets()
        {
            while (true)
            {
                var range = UnityEngine.Random.Range(_levelConfig.TargetSpawnMinDelay, _levelConfig.TargetSpawnMaxDelay);
                yield return new WaitForSeconds(range);

                SpawnNextTarget();
            }
        }

        private void SpawnNextNecessity()
        {
            int safeguard = 100;
            int worldLocationIndex = -1;
            do
            {
                safeguard -= 1;
                if (safeguard <= 0)
                    break;

                worldLocationIndex = _random.Next(_levelConfig.Sources.SpawnPointsLocations.Count);
            } while (_currentWorldLocationIndexes.Contains(worldLocationIndex));

            if (worldLocationIndex == -1)
                return;

            _currentWorldLocationIndexes.Add(worldLocationIndex);

            var radius = 110;
            var thetaRad = (float)_random.NextDouble() * 360;
            var phiRad = (float)_random.NextDouble() * 360;
            var x = radius * Mathf.Cos(thetaRad) * Mathf.Cos(phiRad);
            var z = radius * Mathf.Cos(thetaRad) * Mathf.Sin(phiRad);
            var y = radius * Mathf.Sin(thetaRad);

            var newPosition = new Vector3(x, y, z);

            // var location = _levelConfig.Sources.SpawnPointsLocations[worldLocationIndex];
            // var necessity = Instantiate(_levelConfig.NecessityPrefab, location.Position, location.Rotation, _necessitiesParent);
            var necessity = Instantiate(_levelConfig.NecessityPrefab, newPosition, Quaternion.identity, _necessitiesParent);
            necessity.Init();
        }

        private void SpawnNextTarget()
        {
            int safeguard = 100;
            int worldLocationIndex = -1;
            do
            {
                safeguard -= 1;
                if (safeguard <= 0)
                    break;

                worldLocationIndex = _random.Next(_levelConfig.Targets.SpawnPointsLocations.Count);
            } while (_currentWorldLocationIndexes.Contains(worldLocationIndex));

            if (worldLocationIndex == -1)
                return;

            _currentWorldLocationIndexes.Add(worldLocationIndex);

            var radius = 120;
            var thetaRad = (float)_random.NextDouble() * 360;
            var phiRad = (float)_random.NextDouble() * 360;
            var x = radius * Mathf.Cos(thetaRad) * Mathf.Cos(phiRad);
            var z = radius * Mathf.Cos(thetaRad) * Mathf.Sin(phiRad);
            var y = radius * Mathf.Sin(thetaRad);

            var newPosition = new Vector3(x, y, z);

            // var location = _levelConfig.Sources.SpawnPointsLocations[worldLocationIndex];
            // Instantiate(_levelConfig.TargetPrefab, location.Position, location.Rotation, _targetsParent);
            Instantiate(_levelConfig.TargetPrefab, newPosition, Quaternion.identity, _targetsParent);
        }


        private void OnLevelSuccess()
        {
            SwitchToLevel(_currentLevelIndex + 1);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Configs;
using Game.Entities;
using Game.Sounds;
using Game.World;
using UnityEngine;
using Random = System.Random;

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

        public LevelConfig LevelConfig => _levelConfig;

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
        private Random _random = new Random();
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

            _necessitiesParent = new GameObject("Necessities").transform;
            _targetsParent = new GameObject("Targets").transform;
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

            _timePassed += Time.deltaTime;

            if (_timePassed >= _numSecondsPassed + 1)
            {
                _numSecondsPassed += 1;

                var timeLeft = _levelConfig.Duration - _numSecondsPassed;
                OnTimeChanged?.Invoke(timeLeft);

                if (timeLeft <= 0)
                {
                    _isLevelRunning = false;
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
                StartCoroutine(GameFailureCleanup());
            }
        }

        // Called from GameEvent
        public void StartGame()
        {
            _score = 0;

            SwitchToLevel(0);
        }

        // Called from GameEvent
        public void HelpReceived()
        {
            _score += 100;
            OnScoreChanged?.Invoke(_score);

            // Get XX% of health back
            _currentLevelHealth += _levelConfig.HealthRecoveryPercent * _levelConfig.TotalHealth;
            if (_currentLevelHealth > _levelConfig.TotalHealth)
                _currentLevelHealth = _levelConfig.TotalHealth;
        }

        private void SwitchToLevel(int levelIndex)
        {
            if (levelIndex >= _gameConfig.LevelConfigs.Length)
            {
                StartCoroutine(GameSuccessCleanup());
                return;
            }

            _currentLevelIndex = levelIndex;

            if (_gameConfig.LevelConfigs[_currentLevelIndex] == null)
            {
                throw new Exception($"Undefined level {_currentLevelIndex}");
            }

            CleanUpLevel();
            InitLevel();
        }

        private void CleanUpLevel()
        {
            if (_targetsParent != null)
            {
                for (var i = _targetsParent.childCount - 1; i >= 0; i--)
                {
                    var targetTransform = _targetsParent.GetChild(i);
                    if (targetTransform.TryGetComponent(out Target target))
                    {
                        target.ShrinkAndDestroy();
                    }
                }
            }

            if (_necessitiesParent != null)
            {
                for (var i = _necessitiesParent.childCount - 1; i >= 0; i--)
                {
                    var necessityTransform = _necessitiesParent.GetChild(i);
                    if (necessityTransform.TryGetComponent(out Necessity necessity))
                    {
                        necessity.TriggerPrematureDeath();
                    }
                }
            }

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

        private void CleanUpPlane()
        {
            if (_planeController.CarriedObject != null)
            {
                Destroy(_planeController.CarriedObject.gameObject);
                _planeController.CarriedObject = null;
            }
        }

        private void InitLevel()
        {
            _levelConfig = _gameConfig.LevelConfigs[_currentLevelIndex];

            StartCoroutine(InitLevelCo());
        }

        private IEnumerator InitLevelCo()
        {
            _timePassed = 0f;
            _numSecondsPassed = 0;
            _currentLevelHealth = _levelConfig.TotalHealth;

            // Update UI as soon as possible
            OnTimeChanged?.Invoke(_levelConfig.Duration);
            OnHealthChanged?.Invoke(1, _healthBarGradient.Evaluate(1));

            yield return new WaitForSeconds(0.5f);

            OnShowLevelMessage?.Invoke(_levelConfig.Title);
            yield return new WaitForSeconds(3f);
            OnHideLevelMessage?.Invoke();

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

                // jTODO trying to get position and rotation directly
                // worldLocationIndex = _random.Next(_levelConfig.Sources.SpawnPointsLocations.Count);
                worldLocationIndex = _random.Next(_levelConfig.Sources.SpawnPointsPositions.Count);
            } while (_currentWorldLocationIndexes.Contains(worldLocationIndex));

            if (worldLocationIndex == -1)
                return;

            _currentWorldLocationIndexes.Add(worldLocationIndex);

            /*
            // Alternative: fixing now working saving in SO
            var radius = 110;
            var thetaRad = (float)_random.NextDouble() * 360;
            var phiRad = (float)_random.NextDouble() * 360;
            var x = radius * Mathf.Cos(thetaRad) * Mathf.Cos(phiRad);
            var z = radius * Mathf.Cos(thetaRad) * Mathf.Sin(phiRad);
            var y = radius * Mathf.Sin(thetaRad);

            var newPosition = new Vector3(x, y, z);
            var necessity = Instantiate(_levelConfig.NecessityPrefab, newPosition, Quaternion.identity, _necessitiesParent);
            */

            // Alternative 2
            var position = _levelConfig.Sources.SpawnPointsPositions[worldLocationIndex];
            var rotation = _levelConfig.Sources.SpawnPointsRotations[worldLocationIndex];
            var necessity = Instantiate(_levelConfig.NecessityPrefab, position, rotation, _necessitiesParent);

            // Origin
            // var location = _levelConfig.Sources.SpawnPointsLocations[worldLocationIndex];
            // var necessity = Instantiate(_levelConfig.NecessityPrefab, location.Position, location.Rotation, _necessitiesParent);

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

                // jTODO trying to get position and rotation directly
                // worldLocationIndex = _random.Next(_levelConfig.Targets.SpawnPointsLocations.Count);
                worldLocationIndex = _random.Next(_levelConfig.Targets.SpawnPointsPositions.Count);
            } while (_currentWorldLocationIndexes.Contains(worldLocationIndex));

            if (worldLocationIndex == -1)
                return;

            _currentWorldLocationIndexes.Add(worldLocationIndex);

            /*
             // Alternative: fixing now working saving in SO
            var radius = 120;
            var thetaRad = (float)_random.NextDouble() * 360;
            var phiRad = (float)_random.NextDouble() * 360;
            var x = radius * Mathf.Cos(thetaRad) * Mathf.Cos(phiRad);
            var z = radius * Mathf.Cos(thetaRad) * Mathf.Sin(phiRad);
            var y = radius * Mathf.Sin(thetaRad);

            var newPosition = new Vector3(x, y, z);
            Instantiate(_levelConfig.TargetPrefab, newPosition, Quaternion.identity, _targetsParent);
            */

            // Alternative 2: direct vector and quaternion
            var position = _levelConfig.Sources.SpawnPointsPositions[worldLocationIndex];
            var rotation = _levelConfig.Sources.SpawnPointsRotations[worldLocationIndex];
            Instantiate(_levelConfig.TargetPrefab, position, rotation, _targetsParent);

            // Origin
            // var location = _levelConfig.Sources.SpawnPointsLocations[worldLocationIndex];
            // Instantiate(_levelConfig.TargetPrefab, location.Position, location.Rotation, _targetsParent);
        }


        private void OnLevelSuccess()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.ShowGameSuccess);

            SwitchToLevel(_currentLevelIndex + 1);
        }

        private IEnumerator GameSuccessCleanup()
        {
            yield return new WaitForSeconds(2);

            CleanUpLevel();
            CleanUpPlane();

            OnGameSuccess?.Invoke();
        }

        private IEnumerator GameFailureCleanup()
        {
            CleanUpLevel();
            CleanUpPlane();

            yield return new WaitForSeconds(2);

            OnGameFailure?.Invoke();
        }
    }
}
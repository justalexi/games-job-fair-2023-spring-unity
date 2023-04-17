using System;
using System.Collections;
using Game.Configs;
using UnityEngine;
using UnityEngine.Serialization;

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


        public event Action<float> OnTimeChanged;
        public event Action OnGameFailure;
        public event Action OnGameSuccess;

        private int _currentLevelIndex;
        private float _currentLevelHealth;

        private float _previousTime;
        private float _sceneStartTime;

        // jTODO instantiate it on Earth in front of the camera, tween the latter
        private PlaneController _planeController;


        // private Player

        private void Start()
        {
            // Destroy(EditorOnlyStuff);

            // SwitchToLevel(0);

            _planeController = Instantiate(_planePrefab, Vector3.back * 110, Quaternion.identity);

            _cameraController.Init(_gameConfig, _planeController);
        }

        private void Update()
        {
            var currentTime = Time.time;

            if (currentTime - _previousTime > 1f)
            {
                _previousTime = currentTime;

                var timePassed = currentTime - _sceneStartTime;
                var timeLeft = _gameConfig.LevelConfigs[_currentLevelIndex].Duration - timePassed;
                OnTimeChanged?.Invoke(timeLeft);

                if (timeLeft <= 0)
                {
                    // jTODO uncomment
                    // OnGameFailure?.Invoke();
                }
            }
        }

        /*private void SwitchToLevel(int levelIndex)
        {
            if (levelIndex >= _levels.Length)
            {
                // Show Game Success
                OnGameSuccess?.Invoke();
                return;
            }

            // jTODO clean up previous level

            if (_levels[_currentLevelIndex] != null)
            {
                _levels[_currentLevelIndex].SetActive(false);
            }

            _currentLevelIndex = levelIndex;
            // jTODO show level's title

            if (_levels[_currentLevelIndex] != null)
            {
                _levels[_currentLevelIndex].SetActive(true);

                InitLevel();
            }
        }

        private void InitLevel()
        {
            _numStarsOnCurrentLevel = 0;

            _sceneStartTime = Time.time;

            var stars = FindObjectsOfType<Collectable>();
            if (stars != null)
            {
                _totalStarsOnCurrentLevel = stars.Length;
            }

            var startTransform = _levels[_currentLevelIndex].transform.Find("StartPosition");
            var startPosition = (startTransform != null) ? startTransform.position : Vector3.zero;
            _player.InitLevel(startPosition);

            var cameraPosition = Camera.main.transform.position;
            cameraPosition.x = startPosition.x;
            Camera.main.transform.position = cameraPosition;
        }*/
    }
}
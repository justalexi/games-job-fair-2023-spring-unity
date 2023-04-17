using Game.Configs;
using Game.GameModes.Single;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameState : BaseUIState
    {
        // [SerializeField]
        //  private TextMeshProUGUI _starsText;


        [SerializeField]
        private Button _pauseBtn;

        [SerializeField]
        private TextMeshProUGUI _timerText;

        [SerializeField]
        private TextMeshProUGUI _scoreText;

        [SerializeField]
        private Slider _healthSlider;

        [SerializeField]
        private Image _healthFill;

        [SerializeField]
        private GameObject _messagePanel;

        [SerializeField]
        private TextMeshProUGUI _levelTitle;


        // [SerializeField]
        // private Image[] _playerLives;


        // private Player _player;
        private GameController _gameController;

        // jTODO add pause button if it is present in UI


        public override void Enter()
        {
            base.Enter();

            // _player = FindObjectOfType<Player>();
            // if (_player != null)
            // {
            //     _player.OnNumStarsChanged += OnNumStarsChanged;
            //     _player.OnDamageTaken += OnDamageTaken;
            //     _player.OnDeath += OnDeath;
            // }

            _pauseBtn.onClick.AddListener(OnTogglePause);


            _gameController = FindObjectOfType<GameController>();
            if (_gameController != null)
            {
                _gameController.OnTimeChanged += OnTimeChanged;
                _gameController.OnHealthChanged += OnHealthChanged;
                _gameController.OnScoreChanged += OnScoreChanged;
                _gameController.OnGameFailure += OnGameFailure;
                _gameController.OnGameSuccess += OnGameSuccess;
                _gameController.OnShowLevelMessage += OnShowLevelMessage;
                _gameController.OnHideLevelMessage += OnHideLevelMessage;

                _scoreText.text = $"Score: {_gameController.Score}";
            }

            ControlsReader.Instance.OnTogglePause += OnTogglePause;

            ControlsReader.Instance.EnableGameControls();

            _UIManager.EventSystem.SetSelectedGameObject(null);
        }


        public override void Exit()
        {
            // jTODO Is it ok to disable class before unsubscribing 
            base.Exit();

            // if (_player != null)
            // {
            //     _player.OnNumStarsChanged -= OnNumStarsChanged;
            //     _player.OnDamageTaken -= OnDamageTaken;
            //     _player.OnDeath -= OnDeath;
            // }

            _pauseBtn.onClick.RemoveListener(OnTogglePause);

            if (_gameController != null)
            {
                _gameController.OnTimeChanged -= OnTimeChanged;
                _gameController.OnHealthChanged -= OnHealthChanged;
                _gameController.OnGameFailure -= OnGameFailure;
                _gameController.OnGameSuccess -= OnGameSuccess;
            }

            ControlsReader.Instance.OnTogglePause -= OnTogglePause;
        }

        // private void OnNumStarsChanged(int numStars)
        // {
        //     _starsText.text = numStars.ToString();
        // }

        private void OnTimeChanged(float time)
        {
            var totalSeconds = Mathf.FloorToInt(time);

            var numMinutes = totalSeconds / 60;
            var numMinutesFull = $"{(numMinutes < 10 ? "0" : "")}{numMinutes}";

            var numSeconds = totalSeconds % 60;
            var numSecondsFull = $"{(numSeconds < 10 ? "0" : "")}{numSeconds}";

            _timerText.text = $"<mspace=0.45em>{numMinutesFull}:{numSecondsFull}";
        }

        private void OnScoreChanged(int score)
        {
            _scoreText.text = $"Score: {score}";
        }

        private void OnHealthChanged(float health, Color color)
        {
            _healthSlider.value = health;
            _healthFill.color = color;
        }

        private void OnGameFailure()
        {
            _UIManager.SwitchTo(typeof(GameOverFailureState));
        }

        private void OnGameSuccess()
        {
            _UIManager.SwitchTo(typeof(GameOverSuccessState));
        }


        private void OnShowLevelMessage(string title)
        {
            _levelTitle.text = title;
            _messagePanel.SetActive(true);
        }

        private void OnHideLevelMessage()
        {
            _messagePanel.SetActive(false);
        }


        // private void OnDamageTaken(int numLives)
        // {
        //     for (int i = 0; i < _playerLives.Length; i++)
        //     {
        //         _playerLives[i].enabled = i < numLives;
        //     }
        // }

        // private void OnDeath()
        // {
        //     _UIManager.SwitchTo(typeof(GameOverState));
        // }

        // Pause can be triggered from Controls and UI
        private void OnTogglePause()
        {
            _UIManager.SwitchTo(typeof(PauseState));
        }
    }
}
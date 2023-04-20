using Game.Configs;
using Game.GameModes.Single;
using Game.Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameState : BaseUIState
    {
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


        private GameController _gameController;


        public override void Enter()
        {
            base.Enter();

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

                // Update UI as soon as possible
                OnTimeChanged(_gameController.LevelConfig.Duration);
                OnHealthChanged(_gameController.LevelConfig.TotalHealth, Color.white);
                OnScoreChanged(_gameController.Score);
            }

            ControlsReader.Instance.OnTogglePause += OnTogglePause;

            ControlsReader.Instance.EnableGameControls();

            _UIManager.GameStateEnteredEvent.Raise();

            _UIManager.EventSystem.SetSelectedGameObject(null);
        }

        public override void Exit()
        {
            base.Exit();

            _pauseBtn.onClick.RemoveListener(OnTogglePause);

            if (_gameController != null)
            {
                _gameController.OnTimeChanged -= OnTimeChanged;
                _gameController.OnHealthChanged -= OnHealthChanged;
                _gameController.OnScoreChanged -= OnScoreChanged;
                _gameController.OnGameFailure -= OnGameFailure;
                _gameController.OnGameSuccess -= OnGameSuccess;
                _gameController.OnShowLevelMessage -= OnShowLevelMessage;
                _gameController.OnHideLevelMessage -= OnHideLevelMessage;
            }

            ControlsReader.Instance.OnTogglePause -= OnTogglePause;

            _UIManager.GameStateExitedEvent.Raise();
        }

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
            // _healthFill.color = color;
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

        // Pause can be triggered from Controls and UI
        private void OnTogglePause()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Click);

            _UIManager.SwitchTo(typeof(PauseState));
        }
    }
}
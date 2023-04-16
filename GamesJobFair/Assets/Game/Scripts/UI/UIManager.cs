using System;
using System.Collections.Generic;
using Game.Events;
using Game.Sounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Handling UI States Machine

        private Dictionary<Type, BaseUIState> _states;

        private BaseUIState _currentState;

        [Header("UI State Machine")]
        [SerializeField]
        private MainMenuState _mainMenuState;

        [SerializeField]
        private PauseState _pauseState;

        [SerializeField]
        private GameOverFailureState _gameOverFailureState;

        [SerializeField]
        private GameOverSuccessState _gameOverSuccessState;

        [SerializeField]
        private GameState _gameState;

        [SerializeField]
        private EventSystem _eventSystem;

        public EventSystem EventSystem => _eventSystem;

        #endregion


        #region UI Controls shared by all UI states

        [Header("Sound Controls")]
        [SerializeField]
        private Button _toggleSoundBtn;

        [SerializeField]
        private GameObject _soundOnView;

        [SerializeField]
        private GameObject _soundOffView;

        #endregion

        [Header("Misc")]
        [SerializeField]
        private GameEvent _mainMenuEnteredEvent;

        [SerializeField]
        private GameEvent _mainMenuExitedEvent;

        [SerializeField]
        private GameEvent _quitEvent;


        public GameEvent MainMenuEnteredEvent => _mainMenuEnteredEvent;
        public GameEvent MainMenuExitedEvent => _mainMenuExitedEvent;
        public GameEvent QuitEvent => _quitEvent;


        private void Start()
        {
            _mainMenuState.Init(this);
            _pauseState.Init(this);
            _gameOverFailureState.Init(this);
            _gameOverSuccessState.Init(this);
            _gameState.Init(this);

            _states = new Dictionary<Type, BaseUIState>()
            {
                { typeof(MainMenuState), _mainMenuState },
                { typeof(PauseState), _pauseState },
                { typeof(GameOverFailureState), _gameOverFailureState },
                { typeof(GameOverSuccessState), _gameOverSuccessState },
                { typeof(GameState), _gameState },
            };

            _toggleSoundBtn.onClick.AddListener(OnToggleSound);
            UpdateSoundBtnView();

            SwitchTo(typeof(MainMenuState));
        }

        private void OnDestroy()
        {
            _toggleSoundBtn.onClick.RemoveListener(OnToggleSound);
        }

        private void OnToggleSound()
        {
            // jTODO update button's view
            // jTODO use SoundManager singleton to turn on/off sounds
            SoundManager.Instance.ToggleSounds();
            UpdateSoundBtnView();

            // jTODO use GameEvent to save state
        }

        private void UpdateSoundBtnView()
        {
            var isSoundEnabled = SoundManager.Instance.IsSoundEnabled;
            _soundOnView.SetActive(isSoundEnabled);
            _soundOffView.SetActive(!isSoundEnabled);
        }

        public void SwitchTo(Type newState)
        {
            if (_currentState != null)
                _currentState.Exit();

            _currentState = _states[newState];

            if (_currentState)
                _currentState.Enter();
        }
    }
}
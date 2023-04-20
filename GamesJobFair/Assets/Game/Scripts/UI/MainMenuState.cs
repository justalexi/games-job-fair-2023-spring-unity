using Game.Configs;
using Game.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MainMenuState : BaseUIState
    {
        [SerializeField]
        private Button _startBtn;

        [SerializeField]
        private Button _quitBtn;


        public override void Enter()
        {
            base.Enter();

            Time.timeScale = 0f;

            _startBtn.onClick.AddListener(OnStart);
            _quitBtn.onClick.AddListener(OnQuit);

            ControlsReader.Instance.EnableUIControls();

            _UIManager.EventSystem.SetSelectedGameObject(_startBtn.gameObject);

            _UIManager.MainMenuEnteredEvent.Raise();
        }

        public override void Exit()
        {
            base.Exit();

            _startBtn.onClick.RemoveListener(OnStart);
            _quitBtn.onClick.RemoveListener(OnQuit);

            _UIManager.MainMenuExitedEvent.Raise();

            Time.timeScale = 1f;
        }

        private void OnStart()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Click);

            _UIManager.SwitchTo(typeof(GameState));
        }

        private void OnQuit()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Click);

            _UIManager.QuitEvent.Raise();
        }
    }
}
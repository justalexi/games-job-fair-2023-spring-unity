using Game.Configs;
using Game.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PauseState : BaseUIState
    {
        [SerializeField]
        private Button _unpauseBtn;

        [SerializeField]
        private Button _quitBtn;


        public override void Enter()
        {
            base.Enter();

            Time.timeScale = 0f;

            _unpauseBtn.onClick.AddListener(OnTogglePause);
            _quitBtn.onClick.AddListener(OnQuit);

            ControlsReader.Instance.OnTogglePause += OnTogglePause;

            ControlsReader.Instance.EnableUIControls();

            _UIManager.EventSystem.SetSelectedGameObject(_unpauseBtn.gameObject);

            SoundManager.Instance.PlaySound(SoundManager.Instance.ShowPause);
        }

        public override void Exit()
        {
            // jTODO Is it ok to disable class before unsubscribing 
            base.Exit();

            _unpauseBtn.onClick.RemoveListener(OnTogglePause);
            _quitBtn.onClick.RemoveListener(OnQuit);

            Time.timeScale = 1f;

            ControlsReader.Instance.OnTogglePause -= OnTogglePause;

            SoundManager.Instance.PlaySound(SoundManager.Instance.HidePause);
        }

        // Pause can be triggered from Controls and UI
        private void OnTogglePause()
        {
            _UIManager.SwitchTo(typeof(GameState));
        }

        private void OnQuit()
        {
            _UIManager.QuitEvent.Raise();
        }
    }
}
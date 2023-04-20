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
        }

        public override void Exit()
        {
            base.Exit();

            _unpauseBtn.onClick.RemoveListener(OnTogglePause);
            _quitBtn.onClick.RemoveListener(OnQuit);

            Time.timeScale = 1f;

            ControlsReader.Instance.OnTogglePause -= OnTogglePause;
        }

        // Pause can be triggered from Controls and UI
        private void OnTogglePause()
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
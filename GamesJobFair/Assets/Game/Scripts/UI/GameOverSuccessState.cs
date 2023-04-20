using Game.Configs;
using Game.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOverSuccessState : BaseUIState
    {
        [SerializeField]
        private Button _mainMenuBtn;

        [SerializeField]
        private Button _quitBtn;


        public override void Enter()
        {
            base.Enter();

            Time.timeScale = 0f;

            _mainMenuBtn.onClick.AddListener(OnMainMenu);
            _quitBtn.onClick.AddListener(OnQuit);

            ControlsReader.Instance.EnableUIControls();

            _UIManager.EventSystem.SetSelectedGameObject(_mainMenuBtn.gameObject);
        }

        public override void Exit()
        {
            base.Exit();

            _mainMenuBtn.onClick.RemoveListener(OnMainMenu);
            _quitBtn.onClick.RemoveListener(OnQuit);

            Time.timeScale = 1f;
        }

        private void OnMainMenu()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Click);

            _UIManager.SwitchTo(typeof(MainMenuState));
        }

        private void OnQuit()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Click);

            _UIManager.QuitEvent.Raise();
        }
    }
}
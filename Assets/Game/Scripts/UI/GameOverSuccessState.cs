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

            SoundManager.Instance.PlaySound(SoundManager.Instance.ShowGameSuccess);
        }

        public override void Exit()
        {
            // jTODO Is it ok to disable class before unsubscribing 
            base.Exit();

            _mainMenuBtn.onClick.RemoveListener(OnMainMenu);
            _quitBtn.onClick.RemoveListener(OnQuit);

            Time.timeScale = 1f;
        }

        private void OnMainMenu()
        {
            // jTODO maybe reset game state
            _UIManager.SwitchTo(typeof(MainMenuState));
        }

        private void OnQuit()
        {
            _UIManager.QuitEvent.Raise();
        }
    }
}
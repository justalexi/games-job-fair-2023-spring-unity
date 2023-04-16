using Game.Configs;
using Game.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOverFailureState : BaseUIState
    {
        [SerializeField]
        private Button _restartBtn;

        [SerializeField]
        private Button _quitBtn;


        public override void Enter()
        {
            base.Enter();

            Time.timeScale = 0f;

            _restartBtn.onClick.AddListener(OnRestart);
            _quitBtn.onClick.AddListener(OnQuit);

            ControlsReader.Instance.EnableUIControls();

            _UIManager.EventSystem.SetSelectedGameObject(_restartBtn.gameObject);

            SoundManager.Instance.PlaySound(SoundManager.Instance.ShowGameOver);
        }

        public override void Exit()
        {
            // jTODO Is it ok to disable class before unsubscribing 
            base.Exit();

            _restartBtn.onClick.RemoveListener(OnRestart);
            _quitBtn.onClick.RemoveListener(OnQuit);

            Time.timeScale = 1f;
        }

        private void OnRestart()
        {
            // jTODO maybe reset game state

            _UIManager.SwitchTo(typeof(GameState));

            SoundManager.Instance.PlaySound(SoundManager.Instance.Click);
        }

        private void OnQuit()
        {
            _UIManager.QuitEvent.Raise();
        }
    }
}
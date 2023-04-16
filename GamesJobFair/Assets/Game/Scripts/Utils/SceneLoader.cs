using Game.Configs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Utils
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private GameConfig _gameConfig;


        // NB! Method is called from GameEvent
        public void LoadGameScene()
        {
            var gameSceneName = _gameConfig.GameMode == GameMode.Single ? "Single" : "Multi";
            SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        }
    }
}
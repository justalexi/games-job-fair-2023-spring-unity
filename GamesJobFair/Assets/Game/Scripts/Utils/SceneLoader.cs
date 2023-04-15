using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Utils
{
    public class SceneLoader : MonoBehaviour
    {
        // NB! Method is called from GameEvent
        public void LoadMultiplayerScene()
        {
            /*AsyncOperation asyncLoad = */SceneManager.LoadSceneAsync("Multiplayer", LoadSceneMode.Additive);

            // Wait until the asynchronous scene fully loads
            // while (!asyncLoad.isDone)
            // {
            //     yield return null;
            // }
        }
    }
}
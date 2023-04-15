using UnityEngine;

namespace Game.Utils
{
    public class QuitHelper : MonoBehaviour
    {
        // NB! Method is called from GameEvent
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
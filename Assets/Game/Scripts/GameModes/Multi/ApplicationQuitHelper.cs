using System.Collections;
using UnityEngine;

namespace Game.GameModes.Multi
{
    public class LocalLobby
    {
        public string LobbyID;
    }

    public class LobbyManager
    {
        public void Dispose(){}

        public void LeaveLobbyAsync()
        {
            
        }
    }
    
    public class ApplicationQuitHelper : MonoBehaviour
    {
        private LocalLobby _localLobby;
        private LobbyManager _lobbyManager;


        private void Awake()
        {
            Application.wantsToQuit += OnWantToQuit;
        }

        public void Init(LocalLobby localLobby, LobbyManager lobbyManager)
        {
            _localLobby = localLobby;
            _lobbyManager = lobbyManager;
        }

        private bool OnWantToQuit()
        {
            // jTODO revert
            bool canQuit = string.IsNullOrEmpty(_localLobby?.LobbyID);
            // bool canQuit = string.IsNullOrEmpty(_localLobby?.LobbyID.Value);
            StartCoroutine(LeaveBeforeQuit());
            return canQuit;
        }

        /// <summary>
        /// In builds, if we are in a lobby and try to send a Leave request on application quit, it won't go through if we're quitting on the same frame.
        /// So, we need to delay just briefly to let the request happen (though we don't need to wait for the result).
        /// </summary>
        private IEnumerator LeaveBeforeQuit()
        {
            ForceLeaveAttempt();
            yield return null;
            Application.Quit();
        }

        private void OnDestroy()
        {
            ForceLeaveAttempt();
            _lobbyManager.Dispose();
        }

        private void ForceLeaveAttempt()
        {
            // jTODO revert
            if (!string.IsNullOrEmpty(_localLobby?.LobbyID))
            // if (!string.IsNullOrEmpty(_localLobby?.LobbyID.Value))
            {
// #pragma warning disable 4014
                _lobbyManager.LeaveLobbyAsync();
// #pragma warning restore 4014
                _localLobby = null;
            }
        }
    }
}
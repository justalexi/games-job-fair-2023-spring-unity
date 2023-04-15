using Game.Events;
using Game.Multiplayer.Initialization;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
#if UNITY_EDITOR
using Game.Multiplayer.Local;
#endif

namespace Game.Multiplayer
{
    public class MultiplayerInitializer : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _switchToLocal;

        [SerializeField]
        private bool _isLocal = true;

        public bool IsLocal => _isLocal;

        private string _playerName;


        private void Awake()
        {
            var quitHelperGO = new GameObject("ApplicationQuitHelper");
            quitHelperGO.transform.SetParent(transform);
            // jTODO uncomment and test
            // var quitHelper = quitHelperGO.AddComponent<ApplicationQuitHelper>();
            // quitHelper.Init(localLobby, lobbyManager);
        }

        private void Start()
        {
            _playerName = "Player";

            // jTODO when player clicks "Start" button
            OnStartGame();
        }

        private void OnStartGame()
        {
            if (_isLocal)
            {
                _switchToLocal.Raise();
            }
            else
            {
                InitUnityServices();
            }
        }

        private void InitUnityServices()
        {
#if UNITY_EDITOR
            _playerName = $"{_playerName}{LocalProfileTool.LocalProfileSuffix}";
#endif
            Debug.Log($"{GetType().Name}.Init: _playerName = {_playerName}");

            var servicesInitializer = new ServicesInitializer();
            servicesInitializer.OnFailure += OnServicesInitializerFailure;
            servicesInitializer.OnSuccess += OnServicesInitializerSuccess;
#pragma warning disable CS4014
            servicesInitializer.Init(_playerName);
#pragma warning restore CS4014
        }

        private void OnServicesInitializerFailure()
        {
            Debug.Log($"{GetType().Name}.OnServicesInitializerFailure:");

            SwitchToLocal();
        }

        private void OnServicesInitializerSuccess()
        {
            Debug.Log($"{GetType().Name}.OnServicesInitializerSuccess:");

            InitAuth();
        }

        private void InitAuth()
        {
            var auth = new Auth();
            auth.OnFailure += OnAuthFailure;
            auth.OnSignInSuccess += OnAuthSignInSuccess;
            auth.Authenticate(_playerName);
        }

        private void OnAuthFailure()
        {
            Debug.Log($"{GetType().Name}.OnAuthFailure:");

            SwitchToLocal();
        }

        private void OnAuthSignInSuccess()
        {
            Debug.Log($"{GetType().Name}.OnAuthSignInSuccess:");

            InitLobby();
        }

        private LobbyMediator _lobbyMediator;

        private void InitLobby()
        {
            _lobbyMediator = new LobbyMediator(this);
            _lobbyMediator.OnFailure += OnLobbyFailure;
            _lobbyMediator.OnSuccess += OnLobbySuccess;
            _lobbyMediator.CreateOrJoin();
        }

        private void OnLobbyFailure()
        {
            Debug.Log($"{GetType().Name}.OnLobbyFailure:");
            SwitchToLocal();
        }

        private void OnLobbySuccess()
        {
            Debug.Log($"{GetType().Name}.OnLobbySuccess:");

            InitRelay();
        }

        private void InitRelay()
        {
            // m_setupInGame.StartNetworkedGame(m_LocalLobby, m_LocalUser);

            var relayMediator = new RelayMediator();
            relayMediator.OnFailure += OnRelayFailure;
            relayMediator.OnSuccess += OnRelaySuccess;
            relayMediator.SetRelay(_lobbyMediator);
        }

        private void OnRelayFailure()
        {
            Debug.Log($"{GetType().Name}.OnRelayFailure:");
            SwitchToLocal();
        }

        private void OnRelaySuccess()
        {
            if (AuthenticationService.Instance.PlayerId == _lobbyMediator.HostID)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }

        // jTODO implement and use it when multiplayer breaks down 
        private void SwitchToLocal()
        {
            _isLocal = true;
            if (_switchToLocal != null)
                _switchToLocal.Raise();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Multiplayer
{
    public class LobbyMediator
    {
        const string KeyRelayCode = "RelayCode";

        private readonly MonoBehaviour _coroutineOwner;
        private readonly int _maxPlayers;

        private readonly LobbyEventCallbacks _lobbyEventCallbacks = new LobbyEventCallbacks();

        private Lobby _joinedLobby;

        private Coroutine _joinedLobbyHeartbeat;
        private Coroutine _joinedLobbyUpdatesPoll;


        public event Action OnFailure;
        public event Action OnSuccess;

        public string RelayCode { get; private set; }
        public event Action<string> OnRelayCodeChanged;

        public string HostID { get; private set; }
        public event Action<string> OnHostIDChanged;

        public Lobby JoinedLobby => _joinedLobby;


        public LobbyMediator(MonoBehaviour coroutineOwner, int maxPlayers = 10)
        {
            _coroutineOwner = coroutineOwner;
            _maxPlayers = maxPlayers;
        }

        ~LobbyMediator()
        {
            if (_joinedLobbyHeartbeat != null)
            {
                if (_coroutineOwner != null)
                {
                    _coroutineOwner.StopCoroutine(_joinedLobbyHeartbeat);
                }

                _joinedLobbyHeartbeat = null;
            }

            if (_joinedLobbyUpdatesPoll != null)
            {
                if (_coroutineOwner != null)
                {
                    _coroutineOwner.StopCoroutine(_joinedLobbyUpdatesPoll);
                }

                _joinedLobbyUpdatesPoll = null;
            }
        }


        public async void CreateOrJoin()
        {
            Debug.Log($"{GetType().Name}.CreateOrJoin: ");
            List<Lobby> lobbies = await GetLobbies();
            Debug.Log($"{GetType().Name}.CreateOrJoin: lobbies.Count = {lobbies.Count}");

            if (lobbies.Count > 0)
            {
                JoinLobby(lobbies[0]);
            }
            else
            {
                CreateLobby();
            }
        }

        private async Task<List<Lobby>> GetLobbies()
        {
            Debug.Log($"{GetType().Name}.GetLobbies: ");
            
            try
            {
                // jTODO tweak
                var queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Count = 3,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },
                    // Order = new List<QueryOrder>
                    // {
                    //     new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    // }
                };

                Debug.Log($"{GetType().Name}.GetLobbies: before query");
                
                var queryLobbiesResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                Debug.Log($"{GetType().Name}.GetLobbies: after query");
                

                foreach (var lobby in queryLobbiesResponse.Results)
                {
                    Debug.Log($"{GetType().Name}.ListLobbies: lobby.Name = {lobby.Name} {lobby.MaxPlayers}"); // {lobby.Data["Map"].Value}
                }

                return queryLobbiesResponse.Results;
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log($"{GetType().Name}.ListLobbies: exception = {exception}");
                OnFailure?.Invoke();
            }

            Debug.Log($"{GetType().Name}.ListLobbies: before returning null");
            return null;
        }

        private async void JoinLobby(Lobby lobby)
        {
            try
            {
                _joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);

                Debug.Log($"{GetType().Name}.JoinLobby:");

                PrintPlayers(_joinedLobby);

                HandleJoinLobby();
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log($"{GetType().Name}.JoinLobby: exception = {exception}");
                OnFailure?.Invoke();
            }
        }

        private async void CreateLobby()
        {
            try
            {
                var lobbyName = Guid.NewGuid().ToString("N").Substring(0, 8);

                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, _maxPlayers);

                Debug.Log($"{GetType().Name}.CreateLobby: lobby.Name = {_joinedLobby.Name} {_joinedLobby.MaxPlayers} {_joinedLobby.Id} {_joinedLobby.LobbyCode}");

                PrintPlayers(_joinedLobby);

                HandleJoinLobby();
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log($"{GetType().Name}.CreateLobby: exception = {exception}");
                OnFailure?.Invoke();
            }
        }

        // jTODO maybe implement and in this case switch the game to local
        // private async void LeaveLobby()
        // {
        //     try
        //     {
        //         await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        //     }
        //     catch (LobbyServiceException exception)
        //     {
        //         Debug.Log($"{GetType().Name}.LeaveLobby: exception = {exception}");
        //     }
        // }

        private async void HandleJoinLobby()
        {
            // jTODO what if host is changed?
            if (AuthenticationService.Instance.PlayerId == _joinedLobby.HostId)
            {
                _joinedLobbyHeartbeat = _coroutineOwner.StartCoroutine(LobbyHeartbeat());
            }
            _joinedLobbyUpdatesPoll = _coroutineOwner.StartCoroutine(LobbyUpdatesPoll());

            HostID = _joinedLobby.HostId;
            if (_joinedLobby.Data?.ContainsKey(KeyRelayCode) == true)
            {
                RelayCode = _joinedLobby.Data[KeyRelayCode].Value;
            }

            _lobbyEventCallbacks.DataChanged += changes =>
            {
                foreach (var change in changes)
                {
                    if (change.Key == KeyRelayCode)
                    {
                        var newRelayCode = change.Value.Value.Value;
                        RelayCode = newRelayCode;
                        Debug.Log($"{GetType().Name}.HandleJoinLobby: changed, newRelayCode = {newRelayCode}");

                        OnRelayCodeChanged?.Invoke(newRelayCode);
                    }
                }
            };

            _lobbyEventCallbacks.DataAdded += changes =>
            {
                foreach (var change in changes)
                {
                    if (change.Key == KeyRelayCode)
                    {
                        var newRelayCode = change.Value.Value.Value;
                        RelayCode = newRelayCode;
                        Debug.Log($"{GetType().Name}.HandleJoinLobby: added, newRelayCode = {newRelayCode}");

                        OnRelayCodeChanged?.Invoke(newRelayCode);
                    }
                }
            };

            _lobbyEventCallbacks.DataRemoved += changes =>
            {
                foreach (var change in changes)
                {
                    if (change.Key == KeyRelayCode)
                    {
                        var newRelayCode = "";
                        Debug.Log($"{GetType().Name}.HandleJoinLobby: removed, old RelayCode = {RelayCode}");
                        RelayCode = newRelayCode;

                        OnRelayCodeChanged?.Invoke(newRelayCode);
                    }
                }
            };


            _lobbyEventCallbacks.LobbyChanged += changes =>
            {
                //Lobby Fields
                if (changes.HostId.Changed)
                {
                    var newHostID = changes.HostId.Value;
                    HostID = newHostID;
                    Debug.Log($"{GetType().Name}.HandleJoinLobby: newHostID = {newHostID}");

                    OnHostIDChanged?.Invoke(newHostID);
                }
            };

            try
            {
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(_joinedLobby.Id, _lobbyEventCallbacks);

                OnSuccess?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.Log($"{GetType().Name}.HandleJoinLobby: exception = {exception}");
                OnFailure?.Invoke();
            }
        }
        
        public async void UpdateRelayCode(string relayCode)
        {
            try
            {
                _joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { KeyRelayCode, new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
                    }
                });
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log($"{GetType().Name}.UpdateLobbyMap: exception = {exception}");
            }
        }

        #region Other

        private IEnumerator LobbyHeartbeat()
        {
            // NB! less than 30 seconds 
            yield return new WaitForSeconds(15f);

            SendLobbyHeartbeatRequest();
        }

        private async void SendLobbyHeartbeatRequest()
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
        }

        // Maybe needless, thanks to lobby events
        private IEnumerator LobbyUpdatesPoll()
        {
            // NB! Rate limit is 1 second
            yield return new WaitForSeconds(1.5f);

            PollLobbyForUpdates();
        }

        private async void PollLobbyForUpdates()
        {
            if (_joinedLobby != null)
            {
                _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
            }
        }

        #endregion

        private void PrintPlayers(Lobby lobby)
        {
            Debug.Log($"{GetType().Name}.PrintPlayers: lobby.Name = {lobby.Name}");
            foreach (var player in lobby.Players)
            {
                Debug.Log($"{GetType().Name}.PrintPlayers: player.Id = {player.Id}");
            }
        }
    }
}
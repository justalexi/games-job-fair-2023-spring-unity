using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Game.GameModes.Multi
{
    public class RelayMediator
    {
        private LobbyMediator _lobbyMediator;


        public event Action OnFailure;
        public event Action OnSuccess;


        public async void SetRelay(LobbyMediator lobbyMediator)
        {
            _lobbyMediator = lobbyMediator;

            try
            {
                if (AuthenticationService.Instance.PlayerId == _lobbyMediator.HostID)
                {
                    var joinCode = await SetRelayHostData();
                    if (joinCode == null)
                    {
                        OnFailure?.Invoke();
                    }
                    else
                    {
                        SendRelayCodeToLobby(joinCode);
                    }
                }
                else
                {
                    await AwaitRelayCode(_lobbyMediator);
                    await SetRelayClientData(_lobbyMediator);
                }
            }
            catch (Exception exception)
            {
                Debug.Log($"{GetType().Name}.SetRelay: exception = {exception}");
                OnFailure?.Invoke();
            }
        }

        private async Task<string> SetRelayHostData()
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(_lobbyMediator.JoinedLobby.MaxPlayers);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log($"{GetType().Name}.SetRelayHostData: joinCode = {joinCode}");
                var transport = NetworkManager.Singleton.GetComponentInChildren<UnityTransport>();

                // Alternative 2
                var relayServerData = new RelayServerData(allocation, "dtls");
                transport.SetRelayServerData(relayServerData);

                // Alternative 1
                // transport.SetHostRelayData(
                //     allocation.RelayServer.IpV4,
                //     (ushort)allocation.RelayServer.Port,
                //     allocation.AllocationIdBytes,
                //     allocation.Key,
                //     allocation.ConnectionData
                // );

                // Alternative 3
                // bool isSecure = false;
                // var endpoint = GetEndpointForAllocation(allocation.ServerEndpoints, allocation.RelayServer.IpV4, allocation.RelayServer.Port, out isSecure);
                // transport.SetHostRelayData(AddressFromEndpoint(endpoint), endpoint.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, isSecure);

                return joinCode;
            }
            catch (RelayServiceException exception)
            {
                Debug.Log($"{GetType().Name}.SetRelayHostData: exception = {exception}");
                OnFailure?.Invoke();
            }

            return null;
        }

        private void SendRelayCodeToLobby(string joinCode)
        {
            // This should trigger lobby event and thus change '_lobbyMediator.RelayCode'
            _lobbyMediator.OnRelayCodeChanged += OnRelayCodeChanged;
            _lobbyMediator.UpdateRelayCode(joinCode);
            // maybe needless, see above
            // _lobbyMediator.RelayCode = joinCode;
        }

        private void OnRelayCodeChanged(string joinCode)
        {
            _lobbyMediator.OnRelayCodeChanged -= OnRelayCodeChanged;

            if (!string.IsNullOrEmpty(joinCode))
            {
                OnSuccess?.Invoke();
            }
        }


        private async Task AwaitRelayCode(LobbyMediator lobbyMediator)
        {
            var numRetries = 200;

            while (string.IsNullOrEmpty(lobbyMediator.RelayCode))
            {
                await Task.Delay(100);

                numRetries -= 1;
                if (numRetries <= 0)
                {
                    OnFailure?.Invoke();
                    return;
                }
            }
        }

        private async Task SetRelayClientData(LobbyMediator lobbyMediator)
        {
            try
            {
                Debug.Log($"{GetType().Name}.JoinRelay: lobbyMediator.RelayCode = {lobbyMediator.RelayCode}");

                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobbyMediator.RelayCode);
                var transport = NetworkManager.Singleton.GetComponentInChildren<UnityTransport>();

                // Alternative 1
                // transport.SetClientRelayData(
                //     joinAllocation.RelayServer.IpV4,
                //     (ushort)joinAllocation.RelayServer.Port,
                //     joinAllocation.AllocationIdBytes,
                //     joinAllocation.Key,
                //     joinAllocation.ConnectionData,
                //     joinAllocation.HostConnectionData
                // );

                // Alternative 2
                var relayServerData = new RelayServerData(joinAllocation, "dtls");
                transport.SetRelayServerData(relayServerData);

                OnSuccess?.Invoke();
                // Alternative 3
                // var joinAllocation = await Relay.Instance.JoinAllocationAsync(lobbyMediator.RelayCode);
                // var endpoint = GetEndpointForAllocation(joinAllocation.ServerEndpoints, joinAllocation.RelayServer.IpV4, joinAllocation.RelayServer.Port, out var isSecure);
                // transport.SetClientRelayData(AddressFromEndpoint(endpoint), endpoint.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData, isSecure);
            }
            catch (RelayServiceException exception)
            {
                Debug.Log($"{GetType().Name}.SetRelayClientData: exception = {exception}");
                OnFailure?.Invoke();
            }
        }


        #region Other

        /*
        /// <summary>
        /// Determine the server endpoint for connecting to the Relay server, for either an Allocation or a JoinAllocation.
        /// If DTLS encryption is available, and there's a secure server endpoint available, use that as a secure connection. Otherwise, just connect to the Relay IP unsecured.
        /// </summary>
        NetworkEndPoint GetEndpointForAllocation(List<RelayServerEndpoint> endpoints, string ip, int port, out bool isSecure)
        {
#if ENABLE_MANAGED_UNITYTLS
            foreach (RelayServerEndpoint endpoint in endpoints)
            {
                if (endpoint.Secure && endpoint.Network == RelayServerEndpoint.NetworkOptions.Udp)
                {
                    isSecure = true;
                    return NetworkEndPoint.Parse(endpoint.Host, (ushort)endpoint.Port);
                }
            }
#endif
            isSecure = false;
            return NetworkEndPoint.Parse(ip, (ushort)port);
        }

        string AddressFromEndpoint(NetworkEndPoint endpoint)
        {
            return endpoint.Address.Split(':')[0];
        }
        */

        #endregion
    }
}
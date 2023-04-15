using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Game.Multiplayer.Initialization
{
    public class ServicesInitializer
    {
        public event Action OnFailure;
        public event Action OnSuccess;


        public async Task Init(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                Debug.Log($"{GetType().Name}.Init: Invalid player name: {playerName}");
                OnFailure?.Invoke();
            }

            try
            {
                // The profile name may only contain alphanumeric values, `-`, `_` and have a maximum length of 30 characters.
                await UnityServices.InitializeAsync(new InitializationOptions().SetProfile(playerName));

                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    OnFailure?.Invoke();
                    return;
                }

                OnSuccess?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.Log($"{GetType().Name}.Init: {exception}");
                OnFailure?.Invoke();
            }
        }
    }
}
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Game.GameModes.Multi.Initialization
{
    public class Auth
    {
        public event Action OnFailure;
        public event Action OnSignInSuccess;

        public Auth()
        {
            AuthenticationService.Instance.SignedIn += OnSignedIn;
            AuthenticationService.Instance.SignedOut += OnSignedOut;
            AuthenticationService.Instance.SignInFailed += OnSignedFailed;
            AuthenticationService.Instance.Expired += OnExpired;
        }

        ~Auth()
        {
            AuthenticationService.Instance.SignedIn -= OnSignedIn;
            AuthenticationService.Instance.SignedOut -= OnSignedOut;
            AuthenticationService.Instance.SignInFailed -= OnSignedFailed;
            AuthenticationService.Instance.Expired -= OnExpired;
        }

        public void Authenticate(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                Debug.Log($"{GetType().Name}.Authenticate: Invalid player name: {playerName}");
                OnFailure?.Invoke();
            }

            TrySignIn();
        }

        private async void TrySignIn()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Debug.Log($"{GetType().Name}.TrySignIn:  Is ok?");

                OnSignInSuccess?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.Log($"{GetType().Name}.TrySignIn: {exception}");
                OnFailure?.Invoke();
            }
        }

        private void OnSignedIn()
        {
            Debug.Log($"{GetType().Name}.OnSignedIn: {AuthenticationService.Instance.PlayerId} {AuthenticationService.Instance.PlayerInfo.CreatedAt}");
            // keep it in 'TrySignIn'
            // OnSignInSuccess?.Invoke();
        }

        private void OnSignedOut()
        {
            Debug.Log($"{GetType().Name}.OnSignedOut:");
        }

        private void OnSignedFailed(RequestFailedException obj)
        {
            Debug.Log($"{GetType().Name}.OnSignedFailed:");
        }

        private void OnExpired()
        {
            Debug.Log($"{GetType().Name}.OnExpired:");

            // The authentication service automatically attempts to periodically refresh the player session.
            // If refresh attempts fail before the expiration time, the session expires and raises the Expired event.
            // Developers must handle this case and retry signing in for the player.
            // jTODO sign but without OnSuccess event
            // TrySignIn();
        }
    }
}
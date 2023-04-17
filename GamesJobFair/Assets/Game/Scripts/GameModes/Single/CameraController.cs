using System.Collections;
using Game.Configs;
using UnityEngine;

namespace Game.GameModes.Single
{
    public class CameraController : MonoBehaviour
    {
        private static readonly Quaternion ZeroRotation = Quaternion.Euler(0, 0, 0);

        [SerializeField]
        private Transform _cameraMenuTransform;


        private Coroutine _moveCameraToPlaneCoroutine;
        private Coroutine _moveCameraToMenuPositionCoroutine;
        private PlaneController _planeController;
        private GameConfig _gameConfig;


        public void Init(GameConfig gameConfig, PlaneController planeController)
        {
            _gameConfig = gameConfig;
            _planeController = planeController;
        }


        public void MoveCameraToPlane()
        {
            if (_moveCameraToMenuPositionCoroutine != null)
            {
                StopCoroutine(_moveCameraToMenuPositionCoroutine);
                _moveCameraToMenuPositionCoroutine = null;
            }

            _moveCameraToPlaneCoroutine = StartCoroutine(MoveCameraToPlaneCo());
        }

        public void MoveCameraToMenuPosition()
        {
            if (_moveCameraToPlaneCoroutine != null)
            {
                StopCoroutine(_moveCameraToPlaneCoroutine);
                _moveCameraToPlaneCoroutine = null;
            }

            _moveCameraToMenuPositionCoroutine = StartCoroutine(MoveCameraToMenuPositionCo());
        }


        private IEnumerator MoveCameraToPlaneCo()
        {
            // Late init
            if (_gameConfig == null)
                yield return null;

            // yield return null;
            var cameraTransform = transform;
            cameraTransform.parent = _planeController.CameraPosition;

            while (cameraTransform.localPosition.sqrMagnitude > 1f)
            {
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, Vector3.zero, _gameConfig.CameraSpeed * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, ZeroRotation, _gameConfig.CameraSpeed * Time.deltaTime);
                yield return null;
            }

            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = ZeroRotation;
        }

        private IEnumerator MoveCameraToMenuPositionCo()
        {
            // Late init
            if (_gameConfig == null)
                yield return null;

            var cameraTransform = transform;
            // cameraTransform.parent = _planeController.CameraPosition;
            // cameraTransform.SetParent(null);
            cameraTransform.parent = null;

            while ((cameraTransform.localPosition - _cameraMenuTransform.position).sqrMagnitude > 1f)
                // while (true)
            {
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, _cameraMenuTransform.position, _gameConfig.CameraSpeed * Time.unscaledDeltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, ZeroRotation, _gameConfig.CameraSpeed * Time.unscaledDeltaTime);

                yield return null;
            }

            cameraTransform.localPosition = _cameraMenuTransform.position;
            cameraTransform.localRotation = ZeroRotation;
        }
    }
}
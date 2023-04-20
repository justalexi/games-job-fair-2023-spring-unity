using Game.Configs;
using Game.Entities;
using UnityEngine;

namespace Game.GameModes.Single
{
    public class PlaneController : MonoBehaviour
    {
        [SerializeField]
        private Transform _rotationRoot;

        [SerializeField]
        private Transform _planeView;


        [SerializeField]
        public Transform _cameraPosition;

        public Transform CameraPosition => _cameraPosition;

        [SerializeField]
        private GameConfig _gameConfig;

        private PlaneConfig _planeConfig;
        public int CarriedObjectID;
        public Necessity CarriedObject { get; set; }


        private Rigidbody _rigidbody;
        private Animator _animator;

        private float _friction = 0.98f;
        private float _angularFriction = 0.98f;


        private float _extraSpeed = 0;
        private float _angularSpeed = 0f;


        #region Input

        private bool _isAccelerating;

        #endregion


        private void Awake()
        {
            _planeConfig = _gameConfig.PlaneConfig;

            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Cache inputs
            if (ControlsReader.Instance.AccelerateValue > 0f)
            {
                // jTODO use this flag for vfx and sfx
                _isAccelerating = true;
            }

            if (ControlsReader.Instance.AccelerateValue <= 0f)
            {
                _isAccelerating = false;
            }

            _extraSpeed += _planeConfig.Acceleration * ControlsReader.Instance.AccelerateValue;

            if (_extraSpeed + _planeConfig.DefaultSpeed < _planeConfig.MinSpeed)
                _extraSpeed = _planeConfig.MinSpeed - _planeConfig.DefaultSpeed;
            if (_extraSpeed + _planeConfig.DefaultSpeed > _planeConfig.MaxSpeed)
                _extraSpeed = _planeConfig.MaxSpeed - _planeConfig.DefaultSpeed;

            var finalSpeed = _planeConfig.DefaultSpeed + _extraSpeed;
            var planeViewWorldForward = _planeView.TransformDirection(Vector3.forward);
            _rigidbody.AddForce(planeViewWorldForward * finalSpeed, ForceMode.VelocityChange);

            _extraSpeed *= _friction;

            var rotateValue = ControlsReader.Instance.RotateValue;
            if (rotateValue != 0f)
            {
                _angularSpeed += rotateValue * _planeConfig.AngularAcceleration * Time.deltaTime;
                _rotationRoot.Rotate(0f, _angularSpeed, 0f, Space.Self);
            }

            _angularSpeed *= _angularFriction;

            var turn = Mathf.Clamp(_angularSpeed, -5f, 5f);
            _planeView.localRotation = Quaternion.Euler(0f, 0f, -turn * _planeConfig.TurnMultiplier);

            // Animations
            if (_extraSpeed > _planeConfig.DefaultSpeed)
            {
                _animator.Play("IdleFast");
                // jTODO play sound
            }
            else
            {
                _animator.Play("Idle");
                // jTODO play sound
            }
        }
    }
}
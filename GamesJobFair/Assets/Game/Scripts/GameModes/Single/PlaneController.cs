using System;
using Game.Configs;
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


        private float _fuel;

        private float _timeAccumulator; // was needed to decrease fuel

        // jTODO use GameEvent
        public event Action<int> OnFuelChanged;


        private Rigidbody _rigidbody;
        private Animator _animator;

        private float _friction = 0.98f;
        private float _angularFriction = 0.98f;


        private float _extraSpeed = 0;
        private float _angularSpeed = 0f;


        #region Input

        private bool _isAccelerating;
        private bool _isTurningLeft;
        private bool _isTurningRight;

        #endregion


        private void Awake()
        {
            _planeConfig = _gameConfig.PlaneConfig;

            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // transform.position = 110 * Vector3.up;
        }


        private void Update()
        {
            // Debug.Log($"{GetType().Name}.Update: ControlsReader.Instance.RotateValue = {ControlsReader.Instance.RotateValue}");

            // _timeAccumulator += Time.deltaTime;
            // if (_timeAccumulator > 1)
            // {
            //     _timeAccumulator -= 1;
            //     _fuel.Value -= 1;
            //
            //     OnFuelChanged?.Invoke(_fuel.Value);
            // }

            // if (Input.GetKeyDown(KeyCode.F))
            // {
            //     _fuel.Value += 10;
            //
            //     OnFuelChanged?.Invoke(_fuel.Value);
            // }


            // Cache inputs
            if (ControlsReader.Instance.AccelerateValue > 0f)
            {
                // jTODO use this flag for vfx and sfx
                _isAccelerating = true;
            }

            if (ControlsReader.Instance.AccelerateValue < 0f)
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

            if (ControlsReader.Instance.RotateValue < 0f)
            {
                _isTurningLeft = true;

                _angularSpeed -= _planeConfig.AngularAcceleration * Time.deltaTime;

                _rotationRoot.Rotate(0f, _angularSpeed, 0f, Space.Self);

                _isTurningLeft = false;
            }

            if (ControlsReader.Instance.RotateValue > 0f)
            {
                _isTurningRight = true;

                _angularSpeed += _planeConfig.AngularAcceleration * Time.deltaTime;

                _rotationRoot.Rotate(0f, _angularSpeed, 0f, Space.Self);

                _isTurningRight = false;
            }

            _angularSpeed *= _angularFriction;


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

        /*private void FixedUpdate()
        {
            // jTODO maybe convert "up" from local to world
            if (_isTurningLeft)
            {
                // Debug.Log($"{GetType().Name}.FixedUpdate: left");

                _rigidbody.AddRelativeTorque(Vector3.up * -1 * _rotationSpeed, ForceMode.Acceleration); //AddForce(new Vector2(-_moveForce, 0f), ForceMode2D.Force);
                // _rigidbody.AddRelativeTorque(Vector3.up * -1 * _rotationSpeed, ForceMode.VelocityChange); //AddForce(new Vector2(-_moveForce, 0f), ForceMode2D.Force);

                _isTurningLeft = false;
            }
            else if (_isTurningRight)
            {
                // Debug.Log($"{GetType().Name}.FixedUpdate: right");
                // _rigidbody.AddForce(new Vector2(_moveForce, 0f), ForceMode2D.Force);
                _rigidbody.AddRelativeTorque(Vector3.up * 1 * _rotationSpeed, ForceMode.Acceleration);
                // _rigidbody.AddRelativeTorque(Vector3.up * 1 * _rotationSpeed, ForceMode.VelocityChange);

                _isTurningRight = false;
            }

            Debug.Log($"{GetType().Name}.FixedUpdate: _rigidbody.angularVelocity = {_rigidbody.angularVelocity}");

            // _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
        }*/
    }
}
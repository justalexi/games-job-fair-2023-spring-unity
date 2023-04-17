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


        private float _speed => _planeConfig.Speed;

        private float _rotationSpeed => _planeConfig.RotationSpeed;


        private float _fuel;

        private float _timeAccumulator; // was needed to decrease fuel

        // jTODO use GameEvent
        public event Action<int> OnFuelChanged;


        private Rigidbody _rigidbody;
        private Vector3 _moveAmount;

        private float _angularAcceleration = 3f;
        private float _angularVelocity = 0f;


        #region Input

        private bool _isTurningLeft;
        private bool _isTurningRight;

        #endregion


        private void Awake()
        {
            _planeConfig = _gameConfig.PlaneConfig;

            _rigidbody = GetComponent<Rigidbody>();
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
            if (ControlsReader.Instance.RotateValue < 0f)
            {
                _isTurningLeft = true;

                _angularVelocity -= _angularAcceleration * Time.deltaTime;


                _rotationRoot.Rotate(0f, _angularVelocity, 0f, Space.Self);
                // _rigidbody.AddRelativeTorque(Vector3.up * -1 * _rotationSpeed, ForceMode.Acceleration); //AddForce(new Vector2(-_moveForce, 0f), ForceMode2D.Force);
                // _rigidbody.AddRelativeTorque(Vector3.up * -1 * _rotationSpeed, ForceMode.VelocityChange); //AddForce(new Vector2(-_moveForce, 0f), ForceMode2D.Force);

                // _planeView.localRotation = Quaternion.Euler(0f, 0f, 15f * _rigidbody.angularVelocity.y);

                _isTurningLeft = false;
            }

            if (ControlsReader.Instance.RotateValue > 0f)
            {
                _isTurningRight = true;

                _angularVelocity += _angularAcceleration * Time.deltaTime;

                _rotationRoot.Rotate(0f, _angularVelocity, 0f, Space.Self);

                // _planeView.localRotation = Quaternion.Euler(0f, 0f, -15f * _rigidbody.angularVelocity.y);

                _isTurningRight = false;
            }

            // _planeView.localRotation = Quaternion.Euler(0f, 0f, -15f * _rigidbody.angularVelocity.y);
            // _planeView.localRotation = Quaternion.Euler(0f, 0f, 15f * _angularVelocity);

            // Simulate angularDrag
            _angularVelocity *= 0.98f;


            // transform.Rotate(Vector3.up, Input.GetAxisRaw("Horizontal") * _rotationSpeed, Space.Self);
            // Vector3 moveDir = new Vector3(/*Input.GetAxisRaw("Horizontal")*/0, 0, Input.GetAxisRaw("Vertical")).normalized;
            // _moveAmount = moveDir * _speed;

            // Animations
            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     var animator = GetComponent<Animator>();
            //     animator.Play("Idle");
            // }
            // else if (Input.GetKeyDown(KeyCode.P))
            // {
            //     var animator = GetComponent<Animator>();
            //     animator.Play("IdleFast");
            // }
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
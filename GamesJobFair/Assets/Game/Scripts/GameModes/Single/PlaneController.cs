using System;
using Game.Configs;
using Game.Entities;
using Game.Sounds;
using UnityEngine;

namespace Game.GameModes.Single
{
    public class PlaneController : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _exhaustParticleSystem;

        [SerializeField]
        private ParticleSystem _exhaustGasParticleSystem;

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
        public Necessity CarriedObject { get; set; }


        private Rigidbody _rigidbody;
        private Animator _animator;

        private float _friction = 0.98f;
        private float _angularFriction = 0.98f;


        private float _extraSpeed = 0;
        private float _angularSpeed = 0f;

        private float _particlesChangeSpeed = 0.8f;
        private float _exhauseParticlesMultiplier;
        private ParticleSystem.MainModule _exhauseParticlesMM;
        private ParticleSystem.MainModule _exhauseGasParticlesMM;


        private void Awake()
        {
            _planeConfig = _gameConfig.PlaneConfig;

            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            _rigidbody.isKinematic = true;

            _exhauseParticlesMM = _exhaustParticleSystem.main;
            _exhauseGasParticlesMM = _exhaustGasParticleSystem.main;
        }

        private void Update()
        {
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

            UpdateView();
        }

        private void UpdateView()
        {
            if (ControlsReader.Instance.AccelerateValue > 0f)
            {
                _exhauseParticlesMultiplier = Mathf.Clamp(_exhauseParticlesMultiplier + _particlesChangeSpeed * Time.deltaTime, 1f, 2f);
                _exhauseParticlesMM.startLifetimeMultiplier = _exhauseParticlesMultiplier;
                _exhauseGasParticlesMM.startSizeMultiplier = _exhauseParticlesMultiplier;
                _exhauseGasParticlesMM.startLifetimeMultiplier = _exhauseParticlesMultiplier;

                _animator.Play("IdleFast");
            }

            if (ControlsReader.Instance.AccelerateValue <= 0f)
            {
                _exhauseParticlesMultiplier = Mathf.Clamp(_exhauseParticlesMultiplier - _particlesChangeSpeed * Time.deltaTime, 1f, 2f);
                _exhauseParticlesMM.startLifetimeMultiplier = _exhauseParticlesMultiplier;
                _exhauseGasParticlesMM.startSizeMultiplier = _exhauseParticlesMultiplier;
                _exhauseGasParticlesMM.startLifetimeMultiplier = _exhauseParticlesMultiplier;

                _animator.Play("Idle");
            }
        }

        public void PlaySoundEngine()
        {
            SoundManager.Instance.PlayEngineSound();

            _rigidbody.isKinematic = false;
        }

        public void StopSoundEngine()
        {
            SoundManager.Instance.StopEngineSound();

            _rigidbody.isKinematic = true;
        }
    }
}
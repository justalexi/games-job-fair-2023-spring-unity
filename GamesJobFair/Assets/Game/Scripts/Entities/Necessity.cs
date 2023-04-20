using System;
using DG.Tweening;
using Game.GameModes.Single;
using Game.Gravity;
using Game.Sounds;
using UnityEngine;

namespace Game.Entities
{
    public class Necessity : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private Collider _collider;

        [SerializeField]
        private Collider _innerCollider;

        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private Material _activeMaterial;

        [SerializeField]
        private Material _disabledMaterial;

        [SerializeField]
        private float _totalShowTime;

        [SerializeField]
        private float _totalLifeTime;

        [SerializeField]
        private float _totalSleepTime;

        public Transform ToFollow
        {
            get => _toFollow;
            set => _toFollow = value;
        }

        private Action _currentState;
        private Transform _player;
        private float _showTime;
        private float _lifeTime;
        private float _sleepTime;
        private Transform _toFollow;


        [SerializeField]
        private float _speed = 4;


        private void Update()
        {
            _currentState?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            _toFollow = other.transform;
            var planeController = _toFollow.GetComponent<PlaneController>();
            if (planeController.CarriedObject != null)
            {
                // Player tried to pick up when already carrying something
                _collider.enabled = false;
                _renderer.sharedMaterial = _disabledMaterial;
                _sleepTime = _totalSleepTime;
                _currentState = Sleep;
            }
            else
            {
                // If player can pick it up
                SoundManager.Instance.PlaySound(SoundManager.Instance.Collect);

                planeController.CarriedObject = this;
                _collider.enabled = false;
                _innerCollider.enabled = false;
                Destroy(GetComponent<Attractee>());
                Destroy(_rigidbody);
                _currentState = FollowPlayer;
            }
        }

        public void Init()
        {
            _lifeTime = _totalLifeTime;

            _collider.enabled = false;
            _renderer.sharedMaterial = _disabledMaterial;

            _showTime = _totalShowTime;
            _currentState = Show;
        }

        private void Show()
        {
            _showTime -= Time.deltaTime;

            if (_showTime <= 0)
            {
                _collider.enabled = true;
                _renderer.sharedMaterial = _activeMaterial;
                _currentState = Idle;
            }
        }

        private void Idle()
        {
            _lifeTime -= Time.deltaTime;

            if (_lifeTime <= 0)
            {
                _collider.enabled = false;
                _renderer.sharedMaterial = _disabledMaterial;
                _currentState = Decay;
            }
        }

        private void Sleep()
        {
            _sleepTime -= Time.deltaTime;

            if (_sleepTime <= 0)
            {
                _collider.enabled = true;
                _renderer.sharedMaterial = _activeMaterial;
                _currentState = Idle;
            }
        }

        private void FollowPlayer()
        {
            transform.position = Vector3.Lerp(transform.position, _toFollow.position, _speed * Time.deltaTime);
        }

        private void Decay()
        {
            _currentState = null;

            transform.DOScale(Vector3.zero, 0.7f).OnComplete(() => { Destroy(gameObject); });
        }

        // As soon as this object goes to Idle state, it will switch to Decay. Other states are unaffected
        public void TriggerPrematureDeath()
        {
            _lifeTime = 0;
        }
    }
}
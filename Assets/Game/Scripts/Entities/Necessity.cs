using System;
using System.Collections;
using System.Collections.Generic;
using Game.GameModes.Single;
using Game.Gravity;
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
        private Animator _animator;

        [SerializeField]
        private float _totalShowTime;

        [SerializeField]
        private float _totalLifeTime;

        [SerializeField]
        private float _totalSleepTime;

        // jTODO states (spawn, live a period of time, decay, follow player)

        private Action _currentState;
        private Transform _player;
        private float _showTime;
        private float _lifeTime;
        private float _sleepTime;
        private Queue<Vector3> _positionsToFollow = new Queue<Vector3>();


        private void Start()
        {
        }

        private void Update()
        {
            _currentState?.Invoke();
        }


        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            _player = other.transform;
            var planeController = _player.GetComponent<PlaneController>();
            if (planeController.CarriedObject != null)
            {
                // jTODO play sfx reject, disable collider for X seconds
                _collider.enabled = false;
                _sleepTime = _totalSleepTime;
                _currentState = Sleep;
            }
            else
            {
                // jTODO if empty, then pick up, sfx
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
            _showTime = _totalShowTime;
            _currentState = Show;
        }


        private void Show()
        {
            // jTODO DoTween or vfx/sfx
            _showTime -= Time.deltaTime;

            if (_showTime <= 0)
            {
                _collider.enabled = true;
                _currentState = Idle;
            }
        }


        private void Idle()
        {
            _lifeTime -= Time.deltaTime;

            if (_lifeTime <= 0)
            {
                _collider.enabled = false;
                _currentState = Decay;
            }
        }

        private void Sleep()
        {
            _sleepTime -= Time.deltaTime;

            if (_sleepTime <= 0)
            {
                _collider.enabled = true;
                _currentState = Idle;
            }
        }


        private void FollowPlayer()
        {
            // Let the player fly a bit further
            if (_positionsToFollow.Count < 30)
            {
                _positionsToFollow.Enqueue(_player.position);
                return;
            }

            var position = _positionsToFollow.Dequeue();

            // jTODO DoTween or vfx/sfx
            transform.position = position;
        }

        private void Decay()
        {
            // jTODO DoTween or vfx/sfx

            Destroy(gameObject);
        }
    }
}
using UnityEngine;

namespace Game.Gravity
{
    [RequireComponent(typeof(Rigidbody))]
    public class Attractee : MonoBehaviour
    {
        // Using primitive way of keeping objects on the surface of the planet
        [SerializeField]
        private Vector3 _gravityCenter;

        [SerializeField]
        private float _gravity = -9.81f;


        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Attract(this);
        }

        private void Attract(Attractee attractee)
        {
            var transformCache = transform;
            var direction = (transformCache.position - _gravityCenter).normalized;

            _rigidbody.rotation = Quaternion.FromToRotation(transformCache.up, direction) * transformCache.rotation;

            _rigidbody.AddForce(direction * _gravity);
        }
    }
}
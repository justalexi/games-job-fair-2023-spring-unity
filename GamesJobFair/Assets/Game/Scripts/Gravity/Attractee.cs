using UnityEngine;

namespace Game.Gravity
{
    [RequireComponent(typeof(Rigidbody))]
    public class Attractee : MonoBehaviour
    {
        // jTODO move to SO
        private float _gravity = -9.81f;


        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;

        // Using primitive way of keeping objects on the surface of the planet
        private Vector3 _gravityCenter;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            // jTODO maybe do it in the Inspector
            // _rigidbody.useGravity = false;
            // _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void FixedUpdate()
        {
            Attract(this);
        }

        private void Attract(Attractee attractee)
        {
            var transformCache = transform;
            var direction = (transformCache.position - _gravityCenter).normalized;

            transform.rotation = Quaternion.FromToRotation(transformCache.up, direction) * transformCache.rotation;
            _rigidbody.AddForce(direction * _gravity);
        }
    }
}
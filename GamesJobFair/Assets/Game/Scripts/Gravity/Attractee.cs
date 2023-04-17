using UnityEngine;

namespace Game.Gravity
{
    [RequireComponent(typeof(Rigidbody))]
    public class Attractee : MonoBehaviour
    {
        // Using primitive way of keeping objects on the surface of the planet
        [SerializeField]
        private Vector3 _gravityCenter;


        // jTODO move to SO
        private float _gravity = -9.81f;


        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;


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

            // transform.rotation = Quaternion.FromToRotation(transformCache.up, direction) * transformCache.rotation;
            _rigidbody.rotation = Quaternion.FromToRotation(transformCache.up, direction) * transformCache.rotation;

            var distance = 100f;
            Debug.DrawRay(transformCache.position, distance * transformCache.up, Color.green, 1f);
            Debug.DrawRay(transformCache.position, distance * direction, Color.red, 1f);
            Debug.DrawRay(transformCache.position, distance * transformCache.forward, Color.blue, 1f);
            
            _rigidbody.AddForce(direction * _gravity);
        }
    }
}
using UnityEngine;

namespace Game.Gravity
{
    public class Attractor : MonoBehaviour
    {
        // jTODO move to SO
        private float _gravity = -9.81f;


        public void Attract(Attractee attractee)
        {
            var attracteeTransform = attractee.transform;
            var direction = (attracteeTransform.position - transform.position).normalized;

            attracteeTransform.rotation = Quaternion.FromToRotation(attracteeTransform.up, direction) * attracteeTransform.rotation;
            attractee.Rigidbody.AddForce(direction * _gravity);
        }
    }
}
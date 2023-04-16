using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SphereFollower : MonoBehaviour
    {
        [SerializeField]
        private SphereMovement _sphereMovement;

        public float radius = 8f;

        void Update()
        {
            var x = radius * Mathf.Cos(_sphereMovement.ThetaRad) * Mathf.Cos(_sphereMovement.PhiRad);
            var z = radius * Mathf.Cos(_sphereMovement.ThetaRad) * Mathf.Sin(_sphereMovement.PhiRad);
            var y = radius * Mathf.Sin(_sphereMovement.ThetaRad);

            transform.position = new Vector3(x, y, z);

            transform.LookAt(_sphereMovement.transform.position);
        }
    }
}
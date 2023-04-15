using UnityEngine;

namespace Game.Movement
{
    public class Rotation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _axis;

        [SerializeField]
        private float _angularSpeed;

        // jTODO make it shared
        private float _angle;


        private void Update()
        {
            // _angle = _angularSpeed * Time.deltaTime;
            transform.Rotate(_axis, _angularSpeed * Time.deltaTime);
        }
    }
}
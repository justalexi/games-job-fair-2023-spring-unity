using UnityEngine;

namespace Game.Movement
{
    public class Rotation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _axis;

        [SerializeField]
        private float _angularSpeed;

        [SerializeField]
        private bool _useUnscaledTime;

        private float DeltaTime => _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;


        private void Update()
        {
            // _angle = _angularSpeed * Time.deltaTime;
            transform.Rotate(_axis, _angularSpeed * DeltaTime);
        }
    }
}
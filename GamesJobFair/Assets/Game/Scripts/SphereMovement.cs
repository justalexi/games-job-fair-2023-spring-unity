using UnityEngine;

namespace Game
{
    public class SphereMovement : MonoBehaviour
    {
        public float maxSpeed = 5f;
        public float acceleration = 0.1f;
        public float speed = 0f;
        public float notSpeed = 1f; // Used for determining direction on sphere

        public float turnSpeed = 5f;

        // 0 - 360 
        public float direction = 0f;


        public float radius = 5f;
        public float phi = 0f;
        public float theta = 0f;

        private float _phiRad;
        public float PhiRad => _phiRad;
        private float _thetaRad;
        public float ThetaRad => _thetaRad;

        #region Debug

        public float debugRayLength = 5f;

        #endregion

        void Start()
        {
        }

        void Update()
        {
            var oldPhiRad = Mathf.Deg2Rad * phi;
            var oldThetaRad = Mathf.Deg2Rad * theta;
            var oldX = radius * Mathf.Cos(oldThetaRad) * Mathf.Cos(oldPhiRad);
            var oldZ = radius * Mathf.Cos(oldThetaRad) * Mathf.Sin(oldPhiRad);
            var oldY = radius * Mathf.Sin(oldThetaRad);


            if (Input.GetKey(KeyCode.UpArrow))
            {
                speed += acceleration;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                speed -= acceleration;
            }

            speed = Mathf.Clamp(speed, 0f, maxSpeed);

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction += turnSpeed;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                direction -= turnSpeed;
            }

            direction = ClampAngle(direction);
            var directionRad = Mathf.Deg2Rad * direction;


            #region Direction

            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log($"{GetType().Name}.Update:");
            }

            var phiForDirection = phi; // 0-360
            var deltaPhi2 = Mathf.Cos(directionRad); // 0-1
            phiForDirection += notSpeed * deltaPhi2; // -eternity-+eternity
            phiForDirection = ClampAngle(phiForDirection); // 0-360
            var phiForDirectionRad = Mathf.Deg2Rad * phiForDirection;

            var thetaForDirection = theta;
            var deltaTheta2 = Mathf.Sin(directionRad);
            thetaForDirection += notSpeed * deltaTheta2;
            thetaForDirection = ClampAngle(thetaForDirection);
            var thetaForDirectionRad = Mathf.Deg2Rad * thetaForDirection;

            var directionX = radius * Mathf.Cos(thetaForDirectionRad) * Mathf.Cos(phiForDirectionRad);
            var directionZ = radius * Mathf.Cos(thetaForDirectionRad) * Mathf.Sin(phiForDirectionRad);
            var directionY = radius * Mathf.Sin(thetaForDirectionRad);

            #endregion

            var deltaPhi = Mathf.Cos(directionRad);
            phi += speed * deltaPhi;
            phi = ClampAngle(phi);
            var phiRad = Mathf.Deg2Rad * phi;

            var deltaTheta = Mathf.Sin(directionRad);
            theta += speed * deltaTheta;
            theta = ClampAngle(theta);
            var thetaRad = Mathf.Deg2Rad * theta;

            var x = radius * Mathf.Cos(thetaRad) * Mathf.Cos(phiRad);
            var z = radius * Mathf.Cos(thetaRad) * Mathf.Sin(phiRad);
            var y = radius * Mathf.Sin(thetaRad);

            // var x = radius * Mathf.Sin(thetaRad) * Mathf.Cos(phiRad);
            // var z = radius * Mathf.Sin(thetaRad) * Mathf.Sin(phiRad);
            // var y = radius * Mathf.Cos(thetaRad);
            var newPosition = new Vector3(x, y, z);
            transform.position = newPosition;

            _phiRad = phiRad;
            _thetaRad = thetaRad;

            var lookDirection = new Vector3(directionX - oldX, directionY - oldY, directionZ - oldZ);
            Debug.DrawRay(newPosition, debugRayLength * lookDirection, Color.magenta);
            transform.forward = (lookDirection).normalized;


            // transform.up = transform.position.normalized;
            // jTODO

            // var lookAtPhiRad = Mathf.Deg2Rad * ClampAngle(phi + deltaPhi);
            // var lookAtThetaRad = Mathf.Deg2Rad * ClampAngle(theta + deltaTheta);
            // var lookAt = new Vector3(
            //     1 * Mathf.Cos(lookAtThetaRad) * Mathf.Cos(lookAtPhiRad),
            //     1 * Mathf.Cos(lookAtThetaRad) * Mathf.Sin(lookAtPhiRad),
            //     1 * Mathf.Sin(lookAtThetaRad));

            // transform.forward = (lookAt - newPosition).normalized;
            // transform.LookAt(lookAt);
            // transform.forward = new Vector3(Mathf.Sin(the)) 


            // var dirVector = new Vector3(Mathf.Sin(del))

            // Debug.DrawRay(newPosition, debugRayLength * Vector3.up, Color.green);
            // Debug.DrawRay(newPosition, debugRayLength * Vector3.right, Color.red);
            // Debug.DrawRay(newPosition, debugRayLength * Vector3.forward, Color.blue);


            // var deltaPosition = newPosition - _previousPosition;
            // if (deltaPosition != Vector3.zero)
            // {
            // Debug.Log($"{GetType().Name}.Update: tra = {transform.forward}");
            // }

            // Save for next frame
            _previousPosition = newPosition;
        }

        private Vector3 _previousPosition = Vector3.zero;

        private float ClampAngle(float value)
        {
            while (value > 360f)
            {
                value -= 360f;
            }

            while (value < 0)
            {
                value += 360f;
            }

            return value;
        }
    }
}
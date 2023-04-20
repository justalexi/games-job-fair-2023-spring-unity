using System;
using Game.Events;
using Game.GameModes.Single;
using UnityEngine;

namespace Game.Entities
{
    public class Target : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _helpReceived;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var planeController = other.gameObject.GetComponent<PlaneController>();
                if (planeController.CarriedObject != null)
                {
                    _helpReceived?.Raise();

                    Destroy(gameObject);
                    Destroy(planeController.CarriedObject.gameObject);

                    planeController.CarriedObject = null;
                }
            }
        }
    }
}
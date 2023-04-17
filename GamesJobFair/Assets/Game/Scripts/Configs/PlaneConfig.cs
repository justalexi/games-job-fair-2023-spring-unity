using System;

namespace Game.Configs
{
    [Serializable]
    public class PlaneConfig
    {
        public float Acceleration;
        public float DefaultSpeed;
        public float MinSpeed;
        public float MaxSpeed;
        public float AngularAcceleration;
        public float DashSpeed;

        public float FuelCapacity;
    }
}
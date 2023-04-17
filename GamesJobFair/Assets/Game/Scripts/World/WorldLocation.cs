using System;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public struct WorldLocation
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public WorldLocation(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
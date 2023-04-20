using Game.GameModes.Multi;
using Unity.Collections;
using Unity.Netcode;

namespace Game.World
{
    // public class World : MonoBehaviour
    public class World : NetworkBehaviour
    {
        // jTODO create new stage (Save the whales, the Africa, the Antarctica, etc.)
        // jTODO spawn resources and target(s)
        
        
        public struct WorldData : INetworkSerializable
            // public struct PlaneData : INetworkSerializeByMemcpy
        {
            public int Fuel;

            public int CarriedObjectID;

            // public string Message; // Can NOT use ref types
            public FixedString64Bytes Message; // "Save the whales"

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Fuel);
                serializer.SerializeValue(ref CarriedObjectID);
                // serializer.SerializeValue(ref Message);
            }
        }
        
        #region Network vars

        private NetworkVariable<PlaneController.PlaneData> _planeDataNV = new NetworkVariable<PlaneController.PlaneData>(
            new PlaneController.PlaneData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // jTODO replace with 'PlaneData'
        private NetworkVariable<int> _fuel = new NetworkVariable<int>(
            100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        #endregion
    }
}
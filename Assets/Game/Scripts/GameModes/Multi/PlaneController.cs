using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.GameModes.Multi
{
    // public class PlaneController : MonoBehaviour
    public class PlaneController : NetworkBehaviour
    {
        public struct PlaneData : INetworkSerializable
            // public struct PlaneData : INetworkSerializeByMemcpy
        {
            public int Fuel;

            public int CarriedObjectID;

            // public string Message; // Can NOT use ref types
            public FixedString64Bytes Message;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref Fuel);
                serializer.SerializeValue(ref CarriedObjectID);
                serializer.SerializeValue(ref Message);
            }
        }

        [SerializeField]
        private Transform _spawnedObjectPrefab;

        Transform spawnedObjectTransform;


        // jTODO always fly forward, can accelerate temporarily
        // jTODO speed pickup
        [SerializeField]
        private float _speed = 3f;

        [SerializeField]
        private float _rotationSpeed = 1f;


        private Rigidbody _rigidbody;
        private Vector3 _moveAmount;

        private float _timeAccumulator;

        public event Action<int> OnFuelChanged;

        #region Network vars

        private NetworkVariable<PlaneData> _planeDataNV = new NetworkVariable<PlaneData>(
            new PlaneData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // jTODO replace with 'PlaneData'
        private NetworkVariable<int> _fuel = new NetworkVariable<int>(
            100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        #endregion


        public override void OnNetworkSpawn()
        {
            _fuel.OnValueChanged += OnFuelValueChanged;
        }

        private void OnFuelValueChanged(int value, int newValue)
        {
            Debug.Log($"{GetType().Name}.OnFuelValueChanged: OwnerClientId = {OwnerClientId}." +
                      $" Fuel = {newValue}. " +
                      $"planeData.Fuel = {_planeDataNV.Value.Fuel}. planeData.Message = {_planeDataNV.Value.Message}");

            if (!IsOwner)
                return;

            var planeData = _planeDataNV.Value;
            planeData.Fuel = Mathf.FloorToInt(Time.time);
            planeData.CarriedObjectID = 5;
            planeData.Message = $"Time: {Time.time}";
            _planeDataNV.Value = planeData;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Debug.Log($"{GetType().Name}.Start: OwnerClientId = {OwnerClientId}");

            if (!IsOwner)
                return;

            transform.position = 110 * Vector3.up;
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            _timeAccumulator += Time.deltaTime;

            // if (_timeAccumulator > 1)
            // {
            //     _timeAccumulator -= 1;
            //     _fuel.Value -= 1;
            //
            //     OnFuelChanged?.Invoke(_fuel.Value);
            // }

            if (Input.GetKeyDown(KeyCode.F))
            {
                _fuel.Value += 10;

                OnFuelChanged?.Invoke(_fuel.Value);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                TestServerRpc();
                Test2ServerRpc(42, "HW");
                Test3ServerRpc(new ServerRpcParams());
                TestClientRpc();
                Test2ClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } });
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                TestClientRpc();
                Test2ClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } });
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                // jIMPORTANT Only server can spawn. Use RPC!
                spawnedObjectTransform = Instantiate(_spawnedObjectPrefab);
                spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                if (spawnedObjectTransform != null)
                {
                    // spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
                    Destroy(spawnedObjectTransform.gameObject);
                    spawnedObjectTransform = null;
                }
            }

            transform.Rotate(Vector3.up, Input.GetAxisRaw("Horizontal") * _rotationSpeed, Space.Self);
            Vector3 moveDir = new Vector3(/*Input.GetAxisRaw("Horizontal")*/0, 0, Input.GetAxisRaw("Vertical")).normalized;
            _moveAmount = moveDir * _speed;

            // Animations
            if (Input.GetKeyDown(KeyCode.O))
            {
                var animator = GetComponent<Animator>();
                animator.Play("Idle");
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                var animator = GetComponent<Animator>();
                animator.Play("IdleFast");
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
        }


        // RPC example
        [ServerRpc]
        private void TestServerRpc()
        {
            Debug.Log($"{GetType().Name}.TestServerRpc: ClientOwnerId = {OwnerClientId}");
        }

        [ServerRpc]
        private void Test2ServerRpc(int number, string message) // NB! Can use strings as parameters! (and value types)
        {
            Debug.Log($"{GetType().Name}.TestServerRpc: ClientOwnerId = {OwnerClientId}. {number}. {message}");
        }


        [ServerRpc]
        private void Test3ServerRpc(ServerRpcParams serverRpcParams)
        {
            Debug.Log($"{GetType().Name}.TestServerRpc: ClientOwnerId = {OwnerClientId}. SenderClientId = {serverRpcParams.Receive.SenderClientId}");
        }

        // Meant to run from the server then run on the client
        [ClientRpc]
        private void TestClientRpc()
        {
            Debug.Log($"{GetType().Name}.TestClientRpc: ");
        }

        [ClientRpc]
        private void Test2ClientRpc(ClientRpcParams clientRpcParams)
        {
            Debug.Log($"{GetType().Name}.Test2ClientRpc: ");
        }
    }
}
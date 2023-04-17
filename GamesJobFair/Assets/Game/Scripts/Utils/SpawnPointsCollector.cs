using System.Collections.Generic;
using Game.World;
using UnityEditor;
using UnityEngine;

namespace Game.Utils
{
    [CreateAssetMenu(menuName = "Game/Spawn Points Collector")]
    public class SpawnPointsCollector : ScriptableObject
    {
        [SerializeField]
        private List<WorldLocation> _spawnPointsLocations;

        public List<WorldLocation> SpawnPointsLocations => _spawnPointsLocations;


        // jTODO take into account Earth's rotation
        public void Collect()
        {
            _spawnPointsLocations = new List<WorldLocation>();

            var worldLocations = GameObject.FindGameObjectsWithTag("Location");

            // var worldLocations = FindObjectsOfType<WorldLocation>();
            foreach (var location in worldLocations)
            {
                var worldLocation = new WorldLocation(location.transform.position, location.transform.rotation);
                _spawnPointsLocations.Add(worldLocation);
            }
        }

        public void Clear()
        {
            _spawnPointsLocations.Clear();
        }
    }

    [CustomEditor(typeof(SpawnPointsCollector))]
    public class SpawnPointsCollectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (SpawnPointsCollector)target;

            if (GUILayout.Button("Collect", GUILayout.Height(40)))
            {
                script.Collect();
            }

            if (GUILayout.Button("Clear", GUILayout.Height(40)))
            {
                script.Clear();
            }
        }
    }
}
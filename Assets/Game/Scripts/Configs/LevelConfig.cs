using Game.Entities;
using Game.Utils;
using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        public string Title;

        public float Duration;

        public float DecayRate;

        // jTODO rename (it is state of well-being or smth)
        public float TotalHealth;

        // public Transform[] TargetLocations;
        public SpawnPointsCollector Sources;
        public SpawnPointsCollector Targets;

        // public Transform[] ResourceSpawnLocations;

        public Necessity NecessityPrefab;
        public Target TargetPrefab;
        public float NecessitySpawnMinDelay;
        public float NecessitySpawnMaxDelay;
        public float TargetSpawnMinDelay;
        public float TargetSpawnMaxDelay;
    }
}
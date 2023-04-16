using Game.Entities;
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

        public Transform[] TargetLocations;

        public Transform[] ResourceSpawnLocations;

        public Necessity NecessityPrefab;
    }
}
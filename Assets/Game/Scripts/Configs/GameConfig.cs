using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(menuName = "Game/Game Config")]
    public class GameConfig : ScriptableObject
    {
        public GameMode GameMode;

        public PlaneConfig PlaneConfig;

        public LevelConfig[] LevelConfigs;

        public float CameraSpeed;

        public float Gravity;
    }
}
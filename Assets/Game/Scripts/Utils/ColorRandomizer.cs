using UnityEngine;

namespace Game.Utils
{
    public class ColorRandomizer : MonoBehaviour
    {
        [SerializeField]
        private Material[] _materials;

        private void Start()
        {
            foreach (var material in _materials)
            {
                material.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.5f, 1f);
            }
        }
    }
}
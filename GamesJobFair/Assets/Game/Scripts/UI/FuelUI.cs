using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class FuelUI : MonoBehaviour
    {
        // jTODO on disconnect - make it 'null'
        private PlaneController _planeController;

        [SerializeField]
        private TextMeshProUGUI _fuelText;


        private void Start()
        {
            StartCoroutine(SearchForPlaneController());
        }

        private IEnumerator SearchForPlaneController()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (_planeController == null)
                {
                    var planeController = FindObjectOfType<PlaneController>();
                    if (planeController != null && planeController.IsOwner)
                    {
                        _planeController = planeController;
                        _planeController.OnFuelChanged += OnFuelChanged;
                    }
                }
            }
        }

        private void OnFuelChanged(int fuel)
        {
            _fuelText.text = fuel.ToString();
        }
    }
}
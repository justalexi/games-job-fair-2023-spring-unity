using DG.Tweening;
using Game.Events;
using Game.GameModes.Single;
using Game.Sounds;
using UnityEngine;

namespace Game.Entities
{
    public class Target : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _helpReceived;

        [SerializeField]
        private Transform _viewAnimated;

        [SerializeField]
        private Collider _collider;


        private Sequence _sequence;


        private void Start()
        {
            _sequence = DOTween.Sequence()
                .Append(_viewAnimated.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.3f).SetDelay(0.1f))
                .Append(_viewAnimated.DOScale(Vector3.one, 0.3f).SetDelay(0.1f))
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var planeController = other.gameObject.GetComponent<PlaneController>();
                if (planeController.CarriedObject != null)
                {
                    _helpReceived?.Raise();

                    ShrinkAndDestroy();

                    planeController.CarriedObject.ToFollow = this.transform;
                    planeController.CarriedObject.transform.DOScale(Vector3.zero, 0.7f).OnComplete(() =>
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.Drop);

                        Destroy(planeController.CarriedObject.gameObject);
                        planeController.CarriedObject = null;
                    });
                }
            }
        }

        public void ShrinkAndDestroy()
        {
            _collider.enabled = false;

            _sequence.Kill();

            _viewAnimated.DOScale(Vector3.zero, 1f).OnComplete(() => Destroy(gameObject)).SetEase(Ease.InCubic);
        }
    }
}
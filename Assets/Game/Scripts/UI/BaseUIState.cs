using UnityEngine;

namespace Game.UI
{
    public class BaseUIState : MonoBehaviour
    {
        protected UIManager _UIManager;


        public virtual void Init(UIManager manager)
        {
            _UIManager = manager;
        }

        public virtual void Enter()
        {
            gameObject.SetActive(true);
        }

        public virtual void Exit()
        {
            gameObject.SetActive(false);
        }
    }
}
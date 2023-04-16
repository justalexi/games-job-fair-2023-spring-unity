using System;
using UnityEngine.InputSystem;

namespace Game.Configs
{
    public class ControlsReader
    {
        private static ControlsReader _instance;

        public static ControlsReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ControlsReader();
                }

                return _instance;
            }
        }
        
        public event Action OnTogglePause;
        
        private Controls _controls;


        
        
        private ControlsReader()
        {
            _controls = new Controls();
            
            _controls.Game.Pause.performed += OnTogglePausePerformed;
            _controls.UI.Unpause.performed += OnTogglePausePerformed;

            // _controls.UI.Set
        }

        private void OnTogglePausePerformed(InputAction.CallbackContext context)
        {
            OnTogglePause?.Invoke();
        }

        public void EnableGameControls()
        {
            _controls.UI.Disable();
            _controls.Game.Enable();
        }

        public void EnableUIControls()
        {
            _controls.Game.Disable();
            _controls.UI.Enable();
        }
    }
}
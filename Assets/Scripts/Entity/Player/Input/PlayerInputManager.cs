using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player.Input
{
    public class PlayerInputManager : MonoBehaviour
    {
        public event Action OnInteract;
        public event Action OnAltInteract;

        public event Action OnHeldItemAction;
        
        private PlayerInputAction _input;

        private void Start()
        {
            _input = new PlayerInputAction();
            _input.Gameplay.Enable();

            _input.Gameplay.Interact.performed += OnInteract_Callback;
            _input.Gameplay.AltInteract.performed += OnAltInteract_Callback;
            _input.Gameplay.LeftMouse.performed += OnHeldAction_Callback;
        }
        
        private void OnDestroy()
        {
            _input.Dispose();
        }
        
        private void OnInteract_Callback(InputAction.CallbackContext obj)
        {
            OnInteract?.Invoke();
        }
        
        private void OnAltInteract_Callback(InputAction.CallbackContext obj)
        {
            OnAltInteract?.Invoke();
        }
        
        private void OnHeldAction_Callback(InputAction.CallbackContext obj)
        {
            OnHeldItemAction?.Invoke();
        }

        public Vector2 Dir => _input.Gameplay.Move.ReadValue<Vector2>();
        public Vector2 LookDir => _input.Gameplay.Look.ReadValue<Vector2>();

        public bool Sprint => _input.Gameplay.Sprint.WasPressedThisFrame();  // пока переключение а не зажим
        public bool Jump => _input.Gameplay.Jump.WasPressedThisFrame();  // только нажатие
        public bool JumpSustain => _input.Gameplay.Jump.IsPressed();  // учитывает удержание кнопки
        public bool RotateItem => _input.Gameplay.RotateItem.IsPressed();
        public bool Crouch => _input.Gameplay.Crouch.WasPressedThisFrame();
        public bool LeftClick => _input.Gameplay.LeftMouse.WasPressedThisFrame();
    }
}
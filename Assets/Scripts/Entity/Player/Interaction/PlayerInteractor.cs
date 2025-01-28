using System;
using Entity.Player.Input;
using UnityEngine;

// add state machine?
// detect, none - сделать так чтобы не постоянно вызывалась функция а только при том что состояние меняются 

namespace Entity.Player.Interaction
{
    public class PlayerInteractor : MonoBehaviour, IInteractor
    {
        public event Action<bool> OnDetectSmth;
        public event Action<IInteractable> OnDetectInteractable;
        
        [Header("Entities")]
        [SerializeField] private Transform interactorSource;
        [Header("Interaction params")]
        [SerializeField] private float interactRange = 12f;
        
        public Transform InteractorSource { get; set; }  // стоит ли так делать?

        private PlayerManager _player;
        private PlayerInputManager _input;

        public void Init()
        {
            _player = GetComponent<PlayerManager>();
            _input = _player.Input;
            InteractorSource = interactorSource; // стоит ли так делать?

            _input.OnInteract += HandleInteractions;
            _input.OnAltInteract += HandleAltInteractions;
        }

        public void Update()
        {
            Detect();
        }

        public void HandleInteractions()
        {
            var r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(_player);
                }
            }
        }
        
        // inspect
        public void HandleAltInteractions()
        {
            var r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    interactable.AltInteract(_player);
                }
            }
        }

        private void Detect()
        {
            var r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    OnDetectSmth?.Invoke(true);
                } else
                {
                    OnDetectSmth?.Invoke(false);
                } 
            } else
            {
                OnDetectSmth?.Invoke(false);
            }
        }
    }
}
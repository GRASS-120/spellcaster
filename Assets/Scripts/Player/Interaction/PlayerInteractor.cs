using Player.Input;
using UnityEngine;

namespace Player.Interaction
{
    public class PlayerInteractor : MonoBehaviour, IInteractor
    {
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
            InteractorSource = interactorSource;  // стоит ли так делать?
            
            _input.OnInteract += HandleInteractions;
            _input.OnAltInteract += HandleAltInteractions;
        }


        public void HandleInteractions()
        {
            var r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hit, interactRange))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
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
                    interactable.AltInteract();
                }
            }
        }
    }
}
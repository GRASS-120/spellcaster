using UnityEngine;

namespace Player.Interaction
{
    public interface IInteractor
    {
        public Transform InteractorSource { get; set; }
        
        public void HandleInteractions();
        public void HandleAltInteractions();
    }
}
using UnityEngine;

namespace Entity.Player.Interaction
{
    public interface IInteractor
    {
        public Transform InteractorSource { get; set; }

        public void HandleInteractions();
        public void HandleAltInteractions();
    }
}
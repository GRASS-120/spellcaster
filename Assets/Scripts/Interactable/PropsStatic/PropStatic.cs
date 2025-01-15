using System;
using Player;
using Player.Interaction;
using UnityEngine;

namespace Interactable.PropsStatic
{
    public class PropStatic : MonoBehaviour, IInteractable
    {
        public event Action<PlayerManager> OnPropAction;
        
        public PropStaticSO propData;
        
        public void Interact(PlayerManager player)
        {
            OnPropAction?.Invoke(player);
            // + sound + anim?
            Debug.Log("!");
        }

        public void AltInteract(PlayerManager player)
        {
            Debug.Log("!!!");
        }
    }
}
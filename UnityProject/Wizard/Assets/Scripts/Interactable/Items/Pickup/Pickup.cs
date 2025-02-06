using Entity.Player;
using Player;
using UnityEngine;

namespace Interactable.Items.Pickup
{
    public class Pickup : Item
    {
        public override void HandleAction(PlayerManager player)
        {
            itemManager.StopClipping();
            itemManager.ThrowObject();
        }
    }
}
using Entity.Player;
using Interactable.Items;
using Player;
using Player.Items;
using UnityEngine;

namespace Interactable
{
    [RequireComponent(typeof(Rigidbody))]
    public class Tool : Item
    {
        private ToolSO _toolData;
        
        private void Awake()
        {
            _toolData = (ToolSO) itemData;
        }
        
        private void Start()
        {
            itemManager.OnItemAction += HandleAction;
        }
        
        // public override void Interact(PlayerManager player)
        // {
        //     Debug.Log("!");
        // }

        // public override void AltInteract(PlayerManager player)
        // {
        //     Debug.Log("!!!");
        // }
        
        protected override void HandleAction(PlayerManager player)
        {
            itemManager.StopClipping();
            itemManager.ThrowObject();
        }
    }
}
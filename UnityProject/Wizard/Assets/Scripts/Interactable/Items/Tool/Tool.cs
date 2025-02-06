using Entity.Player;
using Player.Items;
using UnityEngine;

namespace Interactable.Items.Tool
{
    [RequireComponent(typeof(Rigidbody))]
    public class Tool : Item
    {
        private ToolSO _toolData;
        
        private void Awake()
        {
            Debug.Log("!!!");
            _toolData = (ToolSO) itemData;
        }
        
        // private void Start()
        // {
        //     itemManager.OnItemAction += HandleAction;
        // }
        
        // public override void Interact(PlayerManager player)
        // {
        //     Debug.Log("!");
        // }

        // public override void AltInteract(PlayerManager player)
        // {
        //     Debug.Log("!!!");
        // }
        
        public override void HandleAction(PlayerManager player)
        {
            Debug.Log("kinut tool");
            itemManager.StopClipping();
            itemManager.ThrowObject();
        }
    }
}
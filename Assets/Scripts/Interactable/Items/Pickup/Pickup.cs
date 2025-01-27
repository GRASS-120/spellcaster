using Player;

namespace Interactable.Items.Pickup
{
    public class Pickup : Item
    {
        // не делить на подклассы, а сделать интерфейсы????????? что б меньше наследования! 
        
        private void Start()
        {
            itemManager.OnItemAction += HandleAction;
        }

        protected override void HandleAction(PlayerManager player)
        {
            itemManager.StopClipping();
            itemManager.ThrowObject();
        }
    }
}
using Player;

namespace Interactable.Items.Pickup
{
    public class Pickup : Item
    {
        // не делить на подклассы, а сделать интерфейсы????????? что б меньше наследования! 
        
        // большой предмет нужно не кидать, а ставить! мб какая-то проекция нужна, хз, либо точка куда поставится
        // + опустить ниже когда держишь
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
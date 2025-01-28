using Entity.Player;
using Player;
using Player.Items;
using StatsManager;

namespace Interactable.Items.Equipment
{
    public class Equipment : Item, IHasStatModifier
    {
        private EquipmentSO _equipmentData;
        
        private void Awake()
        {
            _equipmentData = (EquipmentSO) itemData;
        }

        private void Start()
        {
            itemManager.OnItemAction += HandleAction;
        }

        public override void Interact(PlayerManager player)
        {
            itemManager.Consume(this);
            _equipmentData.modifiers.ApplyModifierEffect(player, this);
        }

        public StatModifierSO GetStatModifier()
        {
            return _equipmentData.modifiers;
        }
    }
}
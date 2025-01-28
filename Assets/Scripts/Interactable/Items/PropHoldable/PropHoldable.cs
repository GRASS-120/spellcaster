using System.Collections.Generic;
using Entity.Player;
using Player;
using Player.Items;
using StatsManager;
using StatsManager.StatsTypes;
using UnityEngine;

namespace Interactable.Items.PropHoldable
{
    public class PropHoldable : Item, IHasStatModifier
    {
        private PropHoldableSO _propData;
        
        private void Awake()
        {
            _propData = (PropHoldableSO) itemData;
        }
        
        private void Start()
        {
            itemManager.OnItemAction += HandleAction;
            OnPickupItem += OnPickupItem_Callback;
            OnDropItem += OnDropItem_Callback;
        }

        public override void Interact(PlayerManager player)
        {
            if (player.Stance is Stance.Crouch or Stance.Slide) return;

            // нельзя брать PropHoldable на котором ты стоишь
            player.PlayerController.GroundCollider.TryGetComponent<PropHoldable>(out var propHoldable);
            if (propHoldable == this) return;
            
            base.Interact(player);
        }

        protected override void HandleAction(PlayerManager player)
        {
            var modifier = GetStatModifier();
            modifier.DisableModifierEffect(player, this);
            
            itemManager.StopClipping();
            itemManager.ThrowObject();
        }
        
        private void OnPickupItem_Callback(PlayerManager player)
        {
            var modifier = GetStatModifier();
            modifier.ApplyModifierEffect(player, this);
        }
        
        private void OnDropItem_Callback(PlayerManager player)
        {
            var modifier = GetStatModifier();
            modifier.DisableModifierEffect(player, this);
        }

        public StatModifierSO GetStatModifier()
        {
            // когда игрок несет PropHoldable, то его MoveSpeed уменьшается (то, насколько - зависит от массы)
            var modifier = ScriptableObject.CreateInstance<StatModifierSO>();
            var modifierList = new List<StatModifierSO.StatModifierParams>();
            var deceleration = GetComponent<Rigidbody>().mass / 10f;
            
            var decelerationMovementModifier = new StatModifierSO.StatModifierParams
            {
                operationType = StatModifierSO.OperationType.Division,
                type = StatType.MoveSpeed,
                duration = 0f,
                value = deceleration
            };
            
            modifierList.Add(decelerationMovementModifier);
            modifier.statModifierList = modifierList;
            
            return modifier;
        }
    }
}
using System;
using Player;
using Player.Interaction;
using Player.Items;
using UnityEngine;

// кароч, не вижу смысла разделять на классы. лучше оставить конкрутнуб реализацию на код конкретной вещи. в item manager будет куча функций, из них
// + оп логики будет собираться логика шмотки. prop также. то есть для каждой вещи - свой скрипт
// в наследовании нет смысла так как есть IInteractable -> наследование только для конкретных реализаций предмаета Item либо дефолтную


// у предмета должны быть статы! масса, тип (потом от типа будет зависеть какой offset и тп) - возможность разрушить дверь и тп?
// сделать state machine состояний игрока! не только для перемещения, но и для других действий - несет предмет и тп

// TODO: !!! должна быть анимация при подборе - предмет снизу вверх, типо поднял/достал (сделать через lerp и смену offset?)

namespace Interactable.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour, IInteractable, IItemVisitor
    {
        public event Action<PlayerManager> OnDropItem;
        public event Action<PlayerManager> OnPickupItem;
        
        [Header("Entities")]
        public PlayerItemManager itemManager;
        public ItemSO itemData;

        private PlayerManager _player;

        public virtual void Interact(PlayerManager player)
        {
            if (itemManager.HeldItem == null)
            {
                itemManager.PickUpItem(this);
                OnPickupItem?.Invoke(player);
            }
            else
            {
                if (itemManager.CanDropHeldItem)
                {
                    itemManager.StopClipping();  // prevent objects from clipping through walls
                    itemManager.DropObject();
                    OnDropItem?.Invoke(player);
                }
            }
        }

        public virtual void AltInteract(PlayerManager player)
        {
            Debug.Log("alt interaction with " + gameObject.name);
        }

        public void Visit(PlayerManager player)
        {
            _player = player;
        }
        
        protected virtual void HandleAction(PlayerManager player)
        {
            // Debug.Log("override handle action!");
        }
        
        protected virtual void HandleAction()
        {
            // Debug.Log("override handle action!");
        }
    }
}
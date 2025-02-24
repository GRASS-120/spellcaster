﻿using System;
using Entity.Enemy;
using Entity.Player;
using Entity.Player.Interaction;
using Player;
using Player.Items;
using UnityEngine;

// кароч, не вижу смысла разделять на классы. лучше оставить конкрутнуб реализацию на код конкретной вещи. в item manager будет куча функций, из них
// для каждой вещи - свой скрипт
// в наследовании нет смысла так как есть IInteractable -> наследование только для конкретных реализаций предмаета Item либо дефолтную

// сделать state machine состояний игрока! не только для перемещения, но и для других действий - несет предмет и тп

// TODO: !!! должна быть анимация при подборе - предмет снизу вверх, типо поднял/достал (сделать через lerp и смену offset?)

namespace Interactable.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour, IInteractable, IItemVisitor
    {
        public event Action<PlayerManager> OnDropItem;
        public event Action<PlayerManager> OnPickupItem;

        // itemManager OnActionIteem срабатьывает для всех предметов на сцене!!! то есть бросил предмет, а HandleAction 
        // отработал столько рас, сколько предметов на сцене... но при этом с ними ничего не происходит... мб это норм
        // => пофиксил тем, что подписываюсь на OnItemAction в самом PlayerItemManager, причем подписываю только HeldItem
        // => теперь и в Start не нужно у всех подписываться + имеет смысл переписывать HandleAction
        // но при этом OnDropItem и OnPickupItem работает правильно, так как в рамках одного экземпляра вызывается
        
        [Header("Entities")]
        [SerializeField] protected PlayerItemManager itemManager;
        [SerializeField] protected ItemSO itemData;

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
        
        public virtual void HandleAction(PlayerManager player)
        {
        }
    }
}
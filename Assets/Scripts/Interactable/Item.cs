using System;
using Player.Interaction;
using Player.Items;
using UnityEngine;
using Utils;

// хз пока какое разделение будет
// какие вещи для взаимодействия будут?
// 1) вещи которые можно взять в инвентарь (книга, ружие и тп) - item
// 2) элементы окружения для взаимодействия (двери, рычаги и тп) - 
// 3) вещи, которые можно только рассмотреть - pickup

// кароче, пока будут items (их можно на одну кнопку положить в инвентарь, а на другуб осмотреть) и prop (эелементы окружения)

namespace Interactable
{
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour, IInteractable
    {
        [Header("Entities")]
        [SerializeField] private PlayerItemManager items;

        private void Update()
        {
            if (items.HeldItem != null)
            {
                items.MoveObject(); //keep object position at holdPos
                items.RotateObject();
                if (Input.GetKeyDown(KeyCode.Mouse0) && items.CanDropHeldItem == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
                {
                    items.StopClipping();
                    items.ThrowObject();
                }
            }
        }

        public void Interact() //GameObject newObj
        {
            Debug.Log("interaction with " + gameObject.name);

            if (items.HeldItem == null)
            {
                items.PickUpItem(this);
            }
            else
            {
                if (items.CanDropHeldItem)
                {
                    items.StopClipping();  // prevent objects from clipping through walls
                    items.DropObject();
                }
            }
        }

        public void AltInteract()
        {
            Debug.Log("alt interaction with " + gameObject.name);
        }
    }
}
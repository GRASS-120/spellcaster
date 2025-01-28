using System;
using System.Collections.Generic;
using Entity.Player.Input;
using Interactable;
using Interactable.Items;
using Interactable.Items.Pickup;
using Interactable.Items.PropHoldable;
using UnityEngine;
using Utils;

// if hold big pickup -> hide pointer
// когда беру коробку то ей могу легко расталкивать кородки другие...


// 1. не получается в префаб прокинуть this в item

namespace Entity.Player.Interaction
{
    public enum ItemViewMode
    {
        Pickup,
        PropHoldable,
        Tool
    }

    [Serializable] public struct ItemViewData
    {
        public Transform point;
        public Vector3 rotation;
        public Vector3 offset;
        public ItemViewMode mode;
    }
    
    public class PlayerItemManager : MonoBehaviour
    {
        public event Action<PlayerManager> OnItemAction;  // пока пох на props? - нет | нужно пробрасывать функции сюда
        public event Action OnHeldItemChanged;
        
        [Header("Entities")]
        [SerializeField] private PlayerManager player;
        [Header("Interactions params")]
        [SerializeField] private float throwForce = 500f;
        [SerializeField] private float rotationSensitivity = 1f;
        [Header("View params depends on item type")]
        [SerializeField] private List<ItemViewData> itemViewData;
        
        private ItemViewData _currentItemView;
        private PlayerInputManager _input;
        private Item _heldItem;
        private bool _canDropHeldItem = true;
        private Rigidbody _heldItemRb;
        private Collider _heldItemCollider;
        private List<Item> _inventory;
        private LayerMask _holdLayer;
        
        public Item HeldItem => _heldItem;
        public bool CanDropHeldItem => _canDropHeldItem;
        public PlayerInputManager Input => _input;
        public bool HasHeldItem => _heldItem == null;
        
        private void Awake()
        {
            _input = player.Input;
            _holdLayer = LayerMask.NameToLayer(Const.HOLD_LAYER);
            _currentItemView = itemViewData[0];
        }

        private void Start()
        {
            OnHeldItemChanged += HandleItemView;
        }

        public virtual void Update()
        {
            if (_heldItem != null)
            {
                MoveObject(); //keep object position at holdPos

                if (_input.LeftClick && _canDropHeldItem)
                {
                    OnItemAction?.Invoke(player);
                }
            }
        }

        private void HandleItemView()
        {
            if (_heldItem == null) return;
            
            var targetViewMode = ItemViewMode.Pickup;
        
            switch (_heldItem)
            {
                case Pickup pickup:
                {
                    targetViewMode = ItemViewMode.Pickup;
                    break;
                }
                case Tool tool:
                {
                    targetViewMode = ItemViewMode.Tool;
                    break;
                }
                case PropHoldable prob:
                    targetViewMode = ItemViewMode.PropHoldable;
                    break;
            }
        
            var newView = itemViewData.Find(v => v.mode == targetViewMode);
            _currentItemView = newView;
        
            // Можно сразу установить Parent,
            // чтобы потом в MoveObject() только смещение обновлять
            if (_currentItemView.point != null)
            {
                _heldItem.transform.SetParent(_currentItemView.point);
                // Сбросить локальную позицию/вращение — а потом уже применять offset/rotation
                _heldItem.transform.localPosition = Vector3.zero;
                _heldItem.transform.localRotation = Quaternion.identity;
            }
        }

        public void PickUpItem(Item pickUpItem)
        {
            if (pickUpItem.GetComponent<Rigidbody>())
            {
                _heldItem = pickUpItem; 
                _heldItem.transform.localRotation = Quaternion.identity;
                _heldItemRb = pickUpItem.GetComponent<Rigidbody>();
                _heldItemRb.isKinematic = true;
                //_heldItemRb.mass = player.PlayerController.Motor.SimulatedCharacterMass;  // чтобы не расталкивал объекты за счет увеличения массы
                _heldItemRb.transform.parent = _currentItemView.point;
                _heldItemRb.interpolation = RigidbodyInterpolation.Interpolate;
                _heldItem.gameObject.layer = _holdLayer; 
                // make sure object doesnt collide with player, it can cause weird bugs
                //Physics.IgnoreCollision(_heldItem.GetComponent<Collider>(), player.PlayerCollider);
                
                player.Accept(pickUpItem);  // !
                OnHeldItemChanged?.Invoke();
            }
        }

        public void DropObject()
        {
            if (_heldItem == null) return;
            
            //Physics.IgnoreCollision(_heldItem.GetComponent<Collider>(), player.PlayerCollider, true);
            _heldItem.gameObject.layer = 0;
            _heldItemRb.isKinematic = false;
            _heldItem.transform.parent = null; 
            _heldItem = null; 
            OnHeldItemChanged?.Invoke();

        }

        public void MoveObject()
        {
            // Если хотим каждый кадр «притягивать» объект к нужной точке, используем:
            if (_currentItemView.point != null && _heldItem != null)
            {
                // Локальная позиция и поворот
                _heldItem.transform.localPosition = _currentItemView.offset;
                _heldItem.transform.localRotation = Quaternion.Euler(_currentItemView.rotation);
            }
        }
        
        public void RotateObject()
        {
            if (_input.RotateItem)//hold R key to rotate, change this to whatever key you want
            {
                _canDropHeldItem = false; //make sure throwing can't occur during rotating
            
                //disable player being able to look around
                //mouseLookScript.verticalSensitivity = 0f;
                //mouseLookScript.lateralSensitivity = 0f;

                var axisRotation = _input.LookDir * rotationSensitivity;
                
                //rotate the object depending on mouse X-Y Axis
                _heldItem.transform.Rotate(Vector3.down, axisRotation.x);
                _heldItem.transform.Rotate(Vector3.right, axisRotation.y);
            }
            else
            {
                //re-enable player being able to look around
                //mouseLookScript.verticalSensitivity = originalvalue;
                //mouseLookScript.lateralSensitivity = originalvalue;
                _canDropHeldItem = true;
            }
        }

        public void ThrowObject()
        {
            if (_heldItem == null) return;
            
            //same as drop function, but add force to object before undefining it
            //Physics.IgnoreCollision(_heldItem.GetComponent<Collider>(), player.PlayerCollider, false);
            _heldItem.gameObject.layer = 0;
            _heldItemRb.isKinematic = false;
            _heldItem.transform.parent = null;
            _heldItemRb.AddForce(player.Camera.transform.forward * throwForce);
            _heldItem = null;
            OnHeldItemChanged?.Invoke();

        }
        
        public void StopClipping() //function only called when dropping/throwing
        {
            if (_heldItem == null) return;
            
            var clipRange = Vector3.Distance(_heldItem.transform.position, player.Camera.transform.position); //distance from holdPos to the camera
            RaycastHit[] hits;
            hits = Physics.RaycastAll(player.Camera.transform.position, player.Camera.transform.TransformDirection(Vector3.forward), clipRange);
            
            //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
            if (hits.Length > 1)
            {
                //change object position to camera position 
                _heldItem.transform.position = player.Camera.transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
                //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
            }
        }

        public void Consume(Item item)
        {
            player.Accept(item);
            // item.itemData.modifiers.ApplyModifierEffect(player, this);
            Destroy(item.gameObject);
        }
    }
}
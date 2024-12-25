using System;
using System.Collections.Generic;
using Interactable;
using Player.Input;
using R3;
using UnityEngine;
using Utils;

namespace Player.Items
{
    public class PlayerItemManager : MonoBehaviour
    {
        [Header("Entities")]
        [SerializeField] private PlayerManager player;
        [SerializeField] private Transform holdItemPosition;
        [Header("Hold view params")]
        [SerializeField] private float throwForce = 500f;
        [SerializeField] private float rotationSensitivity = 1f;

        private PlayerInputManager _input;
        private Item _heldItem;
        private bool _canDropHeldItem = true;
        private Rigidbody _heldItemRb;
        private List<Item> _inventory;
        private LayerMask _holdLayer;

        public Item HeldItem => _heldItem;
        public bool CanDropHeldItem => _canDropHeldItem;
        
        private void Awake()
        {
            _input = player.Input;
            _holdLayer = LayerMask.NameToLayer(Const.HOLD_LAYER);
        }
        
        public void PickUpItem(Item pickUpItem)
        {
            if (pickUpItem.GetComponent<Rigidbody>())
            {
                _heldItem = pickUpItem; 
                _heldItemRb = pickUpItem.GetComponent<Rigidbody>();
                _heldItemRb.isKinematic = true;
                _heldItemRb.transform.parent = holdItemPosition.transform;
                _heldItem.gameObject.layer = _holdLayer; 
                // make sure object doesnt collide with player, it can cause weird bugs
                Physics.IgnoreCollision(_heldItem.GetComponent<Collider>(), player.PlayerCollider, true);
            }
        }

        public void DropObject()
        {
            Physics.IgnoreCollision(_heldItem.GetComponent<Collider>(), player.PlayerCollider, true);
            _heldItem.gameObject.layer = 0;
            _heldItemRb.isKinematic = false;
            _heldItem.transform.parent = null; 
            _heldItem = null; 
        }

        public void MoveObject()
        {
            //keep object position the same as the holdPosition position
            _heldItem.transform.position = holdItemPosition.transform.position;
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
            //same as drop function, but add force to object before undefining it
            Physics.IgnoreCollision(_heldItem.GetComponent<Collider>(), player.PlayerCollider, false);
            _heldItem.gameObject.layer = 0;
            _heldItemRb.isKinematic = false;
            _heldItem.transform.parent = null;
            _heldItemRb.AddForce(transform.forward * throwForce);
            _heldItem = null;
        }
        
        public void StopClipping() //function only called when dropping/throwing
        {
            var clipRange = Vector3.Distance(_heldItem.transform.position, transform.position); //distance from holdPos to the camera
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
    }
}
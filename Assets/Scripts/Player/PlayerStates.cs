using FiniteStateMachine;
using UnityEngine;

namespace Player
{
    public class GroundedState : IState
    { 
        private readonly PlayerController _controller;

        public GroundedState(PlayerController controller) {
            this._controller = controller;
        }
        
        public void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SlidingState : IState
    { 
        private readonly PlayerController _controller;

        public SlidingState(PlayerController controller) {
            this._controller = controller;
        }
        
        public void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

    public class FallingState : IState
    {
        private readonly PlayerController _controller;

        public FallingState(PlayerController controller) {
            this._controller = controller;
        }
        
        public void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class JumpingState : IState
    {
        private readonly PlayerController _controller;

        public JumpingState(PlayerController controller) {
            this._controller = controller;
        }
        
        public void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class RisingState : IState
    {
        private readonly PlayerController _controller;

        public RisingState(PlayerController controller) {
            this._controller = controller;
        }
        
        public void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}
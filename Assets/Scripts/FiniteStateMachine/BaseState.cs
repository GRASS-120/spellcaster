using Player;
using UnityEngine;

namespace FiniteStateMachine
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController _player;
        protected readonly Animator _animator;

        protected static readonly int LocomotionHash = Animator.StringToHash("");
        protected static readonly int JumpHash = Animator.StringToHash("");

        protected const float CROSS_FADE_DUTATION = 0.1f;

        protected BaseState(PlayerController player, Animator animator)
        {
            this._player = player;
            this._animator = animator;
        }

        
        public virtual void OnEnter()
        {
            // 
        }

        public virtual void Update()
        {
            // 
        }

        public virtual void FixedUpdate()
        {
            // 
        }

        public virtual void OnExit()
        {
            // 
        }
    }
}
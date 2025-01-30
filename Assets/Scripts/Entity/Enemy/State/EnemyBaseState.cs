using FiniteStateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy.State
{
    public abstract class EnemyBaseState : IState
    {
        protected readonly EnemyManager enemy;
        protected readonly Animator animator;
        protected readonly NavMeshAgent agent;
        
        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int WalkHash = Animator.StringToHash("Walk");
        protected static readonly int RunHash = Animator.StringToHash("Run");
        protected static readonly int AttackHash = Animator.StringToHash("Attack");
        protected static readonly int DeathHash = Animator.StringToHash("Death");
        
        protected const float CrossFadeDuration = 0.1f;

        protected EnemyBaseState(EnemyManager enemy, Animator animator, NavMeshAgent agent)
        {
            this.enemy = enemy;
            this.animator = animator;
            this.agent = agent;
        }

        public virtual void OnEnter()
        {
            // nope
        }

        public virtual void Update()
        {
            // nope
        }

        public virtual void FixedUpdate()
        {
            // nope
        }

        public virtual void OnExit()
        {
            // nope
        }
    }
}
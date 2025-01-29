using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy.State
{
    public class EnemyChaseState : EnemyBaseState
    {
        private readonly Transform player;
        
        public EnemyChaseState(EnemyManager enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator, agent)
        {
            this.player = player;
        }

        public override void OnEnter()
        {
            Debug.Log("chase");
            animator.CrossFade(RunHash, CrossFadeDuration);
        }

        public override void Update()
        {
            agent.SetDestination(player.position);
        }
    }
}
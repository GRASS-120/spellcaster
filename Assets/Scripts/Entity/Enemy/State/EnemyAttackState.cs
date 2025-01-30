using Entity.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy.State
{
    public class EnemyAttackState : EnemyBaseState
    {
        private readonly PlayerManager _player;
        
        public EnemyAttackState(EnemyManager enemy, Animator animator, NavMeshAgent agent, PlayerManager player) : base(enemy, animator, agent)
        {
            _player = player;
        }

        public override void OnEnter()
        {
            Debug.Log("attack");
            animator.CrossFade(AttackHash, CrossFadeDuration);
        }

        public override void Update()
        {
            // agent.ResetPath();
            // agent.SetDestination(_player.Position);
            agent.SetDestination(_player.Position);
            enemy.Attack();
            
        }
    }
}
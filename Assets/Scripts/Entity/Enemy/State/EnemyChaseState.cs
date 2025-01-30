using Entity.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy.State
{
    public class EnemyChaseState : EnemyBaseState
    {
        private readonly PlayerManager _player;
        
        public EnemyChaseState(EnemyManager enemy, Animator animator, NavMeshAgent agent, PlayerManager player) : base(enemy, animator, agent)
        {
            _player = player;
        }

        public override void OnEnter()
        {
            Debug.Log("chase");
            animator.CrossFade(RunHash, CrossFadeDuration);
        }

        public override void Update()
        {
            agent.SetDestination(_player.Position);
        }
    }
}
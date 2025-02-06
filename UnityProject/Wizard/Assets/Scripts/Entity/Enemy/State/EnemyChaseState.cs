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
            // TODO: СДЕЛАТЬ СКОРОСТЬ СПРИНТА В СТАТАХ
            Debug.Log("chase");
            
            var tmp = agent.speed;
            agent.speed = tmp * 2;
            
            animator.CrossFade(RunHash, CrossFadeDuration);
        }

        public override void Update()
        {
            agent.SetDestination(_player.Position);
        }

        public override void OnExit()
        {
            // TODO: СДЕЛАТЬ СКОРОСТЬ СПРИНТА В СТАТАХ
            var tmp = agent.speed;
            agent.speed = tmp / 2;
        }
    }
}
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
            
            agent.isStopped = true;  // враг не двигается в момент атаки. сначала атака, потом если игрок вышел из зоны - переход в ChaseState
            animator.CrossFade(AttackHash, CrossFadeDuration);
        }

        public override void Update()
        {
            // приходится поворачиваем вручную, так как если агент не движется, то он и не поворачивается
            var dirToPlayer = _player.Position - enemy.transform.position;
            dirToPlayer.y = 0;
            if (dirToPlayer != Vector3.zero)
            {
                enemy.transform.rotation = Quaternion.Slerp(
                    enemy.transform.rotation,
                    Quaternion.LookRotation(dirToPlayer),
                    5f * Time.deltaTime
                );
            }

            enemy.HandleAttack(_player);
        }
        
        public override void OnExit()
        {
            Debug.Log("exit attack");
            agent.isStopped = false;
        }
    }
}
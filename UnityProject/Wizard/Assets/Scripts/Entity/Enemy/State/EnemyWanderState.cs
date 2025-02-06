using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy.State
{
    public class EnemyWanderState : EnemyBaseState  // TODO: нужно делать дочерные стейты - отдельно для EnemyState, отдельно для PropToggle и тп 
    {
        private readonly float _wanderRadius;
        private readonly Vector3 _startPoint;
        
            
        public EnemyWanderState(EnemyManager enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator, agent)
        {
            _wanderRadius = wanderRadius;
            _startPoint = enemy.transform.position;
        }

        public override void OnEnter()
        {
            // TODO: мб сделать задержку на 1-5 сек после того, как враг пришел в место назначение? в idle что б был. но это позже 
            animator.CrossFade(WalkHash, CrossFadeDuration);
        }

        public override void Update()
        {
            if (HasReachedDestination())
            {
                // находим случайную точки в пределах _wanderRadius и делаем ее местом назначения
                var randomPoint = Random.insideUnitSphere * _wanderRadius;
                randomPoint += _startPoint;
                
                NavMesh.SamplePosition(randomPoint, out var hit, _wanderRadius, 1);
                var finalPosition = hit.position;

                agent.SetDestination(finalPosition);
            }
        }

        private bool HasReachedDestination()
        {
            // путь не расчитывается в данный момент И
            // агент вошел в радиус, в котором точка считается достигнутой И (stoppingDistance нужен, чтобы враг не входил в игрока, а был рядом с ним)
            // (агент стоит на месте ИЛИ нет муршрута)
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                   (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}
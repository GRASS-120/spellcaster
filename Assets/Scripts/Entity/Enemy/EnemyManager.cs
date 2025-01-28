using System;
using FiniteStateMachine;
using StatsManager;
using StatsManager.StatsTypes;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyManager : MonoBehaviour, IEntity, IHasState
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private BaseStats baseStats;
        
        // потом перенести в другой скрипт
        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int WalkHash = Animator.StringToHash("Walk");
        protected static readonly int RunHash = Animator.StringToHash("Run");
        protected const float CrossFadeDuration = 0.1f;
        
        public Stats Stats { get; set; }

        private StateMachine _stateMachine;

        private void Awake()
        {
            Stats = new Stats(new StatsMediator(), baseStats);
            _stateMachine = new StateMachine();
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        public void At(IState from, IState to, IPredicate condition) =>
            _stateMachine.AddTransition(from, to, condition);

        public void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        public class EnemyWanderState : BaseState  //TODO: нужно делать дочерные стейты - отдельно для EnemyState, отдельно для PropToggle и тп 
        {
            private EnemyManager _enemy;
            private Animator _animator;
            private NavMeshAgent _agent;
            private float _wanderRadius;
            private Vector3 _startPoint;
            
            public EnemyWanderState(EnemyManager enemy, Animator animator, NavMeshAgent agent, float wanderRadius, Vector3 startPoint)
            {
                _enemy = enemy;
                _animator = animator;
                _agent = agent;
                _wanderRadius = wanderRadius;
                _startPoint = startPoint;
            }

            public override void OnEnter()
            {
                _animator.CrossFade(WalkHash, CrossFadeDuration);
            }

            public override void Update()
            {
                
            }

            private bool HasReachedDestination()
            {
                // путь не расчитывается в данный момент И
                // агент вошел в радиус, в котором точка считается достигнутой И (stoppingDistance нужен, чтобы враг не входил в игрока, а был рядом с ним)
                // (агент стоит на месте ИЛИ нет муршрута)
                return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance &&
                       (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
            }
        }
    }
}
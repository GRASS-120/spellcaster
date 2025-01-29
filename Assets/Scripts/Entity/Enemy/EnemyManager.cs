using System;
using Entity.Enemy.State;
using FiniteStateMachine;
using StatsManager;
using StatsManager.StatsTypes;
using UnityEngine;
using UnityEngine.AI;

// TODO: пока переходы между анимациями контролируются через код, а не через аниматор. потом нверное нужно исправить... или так лучше?

namespace Entity.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyManager : MonoBehaviour, IEntity, IHasState
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private BaseStats baseStats;
        [SerializeField] private float wanderRadius = 10f;

        public Stats Stats { get; set; }

        private StateMachine _stateMachine;

        private void Start()
        {
            Stats = new Stats(new StatsMediator(), baseStats);
            _stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);

            Any(wanderState, new FuncPredicate(() => true)); 
            
            
            _stateMachine.SetState(wanderState);
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        public void At(IState from, IState to, IPredicate condition) =>
            _stateMachine.AddTransition(from, to, condition);
        
        public void Any(IState to, IPredicate condition) =>
            _stateMachine.AddAnyTransition(to, condition);
    }
}
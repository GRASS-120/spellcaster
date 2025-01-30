using System;
using Entity.Enemy.PlayerDetection;
using Entity.Enemy.State;
using FiniteStateMachine;
using StatsManager;
using StatsManager.StatsTypes;
using UnityEngine;
using UnityEngine.AI;
using Utils;

// TODO: пока переходы между анимациями контролируются через код, а не через аниматор. потом нверное нужно исправить... или так лучше?

namespace Entity.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class EnemyManager : MonoBehaviour, IEntity, IHasState
    {
        [Header("Entities")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerDetector playerDetector;
        [Header("Params")]
        [SerializeField] private BaseStats baseStats;
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float timeBetweenAttacks = 1f;

        public Stats Stats { get; set; }

        private StateMachine _stateMachine;
        private CountdownTimer _attackTimer;

        private void Start()
        {
            Stats = new Stats(new StatsMediator(), baseStats);  // TODO: СДЕЛАТЬ СКОРОСТЬ АГЕНТА ТАКОЙ ЖЕ КАКАЯ И В СТАТАХ!
            _stateMachine = new StateMachine();
            _attackTimer = new CountdownTimer(timeBetweenAttacks);

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);

            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer())); 
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

            _stateMachine.SetState(wanderState);
        }

        private void Update()
        {
            _stateMachine.Update();
            _attackTimer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        // TODO: ПОЧЕМУ-ТО Attack вызывается, но атака происходит только один раз! 
        // TODO: при том, что в видосе у автора все работает. а по идее не должно, ведь атака происходит только при входе
        // TODO: в состояние атаки! чзх
        // TODO: есть предположение, что это связано с радиусом атаки - когда я за него выхожу (но не выхожу из зоны преследования, он опять атакует...
        public void Attack()
        {
            if (_attackTimer.IsRunning) return;

            _attackTimer.Start();
            
            Debug.Log("attacking");
        }

        public void At(IState from, IState to, IPredicate condition) =>
            _stateMachine.AddTransition(from, to, condition);
        
        public void Any(IState to, IPredicate condition) =>
            _stateMachine.AddAnyTransition(to, condition);
    }
}
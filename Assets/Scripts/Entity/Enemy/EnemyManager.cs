using System;
using Entity.Enemy.PlayerDetection;
using Entity.Enemy.State;
using Entity.Player;
using FiniteStateMachine;
using StatsManager;
using StatsManager.StatsTypes;
using UnityEngine;
using UnityEngine.AI;
using Utils;

// TODO: пока переходы между анимациями контролируются через код, а не через аниматор. потом нверное нужно исправить... или так лучше?
// TODO: вот мне что не нравится в гайде: он использует стейт машину только для анимаций... мне кажется, что лучше
// TODO: 1) анимации вынести в отдельный скрипт и сделать с аниматором, а не через код 
// TODO: 2) использовать стейт машину именно для логики, а не анимаций (?????) хотя если добавить в Attack вызов функции для
// TODO: нанесения урона, то уже и не путаница получается... хм... надо бы спросить
// TODO: исправить кринж-повороты в анимациях?...

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

        public Stats Stats { get; set; }

        private StateMachine _stateMachine;
        private CountdownTimer _attackTimer;

        private void Start()
        {
            Stats = new Stats(new StatsMediator(), baseStats); 
            _stateMachine = new StateMachine();
            _attackTimer = new CountdownTimer(Stats.AttackSpeed);

            agent.speed = Stats.MoveSpeed;

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);

            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer())); 
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));

            // !playerDetector.CanAttackPlayer() && !_attackTimer.IsRunning - атакует если игрок в зоне атаки + не прерывает атаку, когда
            // игрок выходит из зоны, а доводит ее до конца сначала, а потом мереходит в ChaseState
            // || !_attackTimer.IsRunning - отвечает за то, чтобы враг атаковал несколько раз. без этого враг атакует один раз - при заходе в зону атаки
            At(attackState, chaseState, new FuncPredicate(
                () => !playerDetector.CanAttackPlayer() && !_attackTimer.IsRunning || !_attackTimer.IsRunning));
            
            // то есть логика следубющая: враг видит игрока, преследует его, когда игрок оказывается досигаем для атаки - атакует. 
            // после атаки он переходит в ChaseState. и так по кругу (возврат нужен, так как враг атакует только при входе в AttackState)

            // толкчки убрал так - сделал stopping distance = attack radius
            // также убрал коллайдер у меча - сделал просто триггером
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
        
        public void HandleAttack(PlayerManager player)
        {
            // если враг атакует в текущий момент, то скип
            if (_attackTimer.IsRunning) return;

            // если атаки нет, то запускаем таймер
            _attackTimer.Start();
            
            if (playerDetector.CanDamagePlayer()) 
                player.TakeDamage(Stats.AttackDamage);

            Debug.Log("attacking");
        }

        public void At(IState from, IState to, IPredicate condition) =>
            _stateMachine.AddTransition(from, to, condition);
        
        public void Any(IState to, IPredicate condition) =>
            _stateMachine.AddAnyTransition(to, condition);
    }
}
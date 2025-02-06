using System;
using Entity.Player;
using UnityEngine;
using Utils;

namespace Entity.Enemy.PlayerDetection
{
    public class PlayerDetector : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private float detectionAngle = 60f;  // cone in front of the enemy
        [SerializeField] private float detectionRadius = 10f;  // large circle around enemy
        [SerializeField] private float innerDetectionRadius = 5f;  // small circle around enemy
        [SerializeField] private float detectionCooldown = 1f;  // time between detections
        [SerializeField] private float attackRange = 2f;
        
        public PlayerManager Player { get; private set; }
        
        private CountdownTimer _detectionTimer;
        private IDetectionStrategy _detectionStrategy;

        private void Start()
        {
            _detectionTimer = new CountdownTimer(detectionCooldown);
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();  // TODO: нужно спросить: норм ли использовать теги?
            _detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
        }

        private void Update()
        {
            _detectionTimer.Tick(Time.deltaTime);
        }

        public bool CanDetectPlayer()
        {
            return _detectionTimer.IsRunning || _detectionStrategy.Execute(Player, transform, _detectionTimer);
        }

        public bool CanAttackPlayer()
        {
            var dirToPlayer = Player.Position - transform.position;
            return dirToPlayer.magnitude <= attackRange;
        }

        // TODO: нужно доработать логику: сейчас он наносит урон в самом начале атаки (когда только замах начинается)
        // и увернуться получается только если справа обойти. 
        // нужно чтобы он проверял урон в момент, когда будет сам удар. как это сделать? вроде у анимаций можно тригерры ставить и на
        // них события вешать, чтобы когда анимация достигает опред момента - обработать событие
        public bool CanDamagePlayer()
        {
            var dirToPlayer = Player.Position - transform.position;
    
            if (!CanAttackPlayer())
                return false;
    
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            // Если угол меньше порогового – игрок находится перед врагом.
            return (angleToPlayer <= detectionAngle / 2f) ;
        }

        public void SetDetectionStrategy(IDetectionStrategy detectionStrategy) =>
            _detectionStrategy = detectionStrategy;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            
            // draw a spheres 
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // cone dirs
            // делим угол на два, так как detectionAngle = весь угол, но удобнее отступать от центра влево/право => берем половину угла
            // (60' - весь угол = 30' влево + 30' вправо)
            Vector3 rightConeDir = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
            Vector3 leftConeDir = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;
 
            // draw lines to represent cone
            Gizmos.DrawLine(transform.position, transform.position + rightConeDir);
            Gizmos.DrawLine(transform.position, transform.position + leftConeDir);
        }
    }
}
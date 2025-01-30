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
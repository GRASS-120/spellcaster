using UnityEngine;
using Utils;

namespace Entity.Enemy
{
    public class PlayerDetector : MonoBehaviour
    {
        [SerializeField] private float detectionAngle = 60f;  // cone in front of the enemy
        [SerializeField] private float detectionRadius = 10f;  // large circle around enemy
        [SerializeField] private float innerDetectionRadius = 5f;  // small circle around enemy
        // add chaseRadius (чекай тз) - detectionRadius?
        [SerializeField] private float detectionCooldown = 1f;  // time between detections
        
        public Transform Player { get; private set; }
        
        private CountdownTimer _detectionTimer;
        private IDetectionStrategy _detectionStrategy;
    }
}
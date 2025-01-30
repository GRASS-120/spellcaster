using Entity.Player;
using UnityEngine;
using Utils;

namespace Entity.Enemy.PlayerDetection
{
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        private readonly float _detectionAngle;
        private readonly float _detectionRadius;
        private readonly float _innerDetectionRadius;

        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
        {
            _detectionAngle = detectionAngle;
            _detectionRadius = detectionRadius;
            _innerDetectionRadius = innerDetectionRadius;
        }

        public bool Execute(PlayerManager player, Transform detector, CountdownTimer timer)
        {
            if (timer.IsRunning) return false;  // детекшн в кд

            var dirToPlayer = player.Position - detector.position;
            
            // в чем сложность для понимания была: в PlayerDetector.OnDrawGizmos я рисую конус отступая от центра влево и вправо на половину угла
            // но тут тоже половина, но проверяется только (+) -> не было понятно, как проверяется левая сторона. а тут суть в другом:
            // Vector3.Angle возвращает абсолютное значение (сколько градусов МЕЖДУ dirToPlayer и detector.forward),
            // а в OnDrawGizmos я использовал относительное (относительно forward СЛЕВА и СПРАВА, поэтому (-) и (+) нужен) =>
            // достаточно проверить только одну сторону, так как что игрок справа, что игрок слева, все равно угол нужен один - 30'
            // (если не понял - нарисуй)
            var angleToPlayer = Vector3.Angle(dirToPlayer, detector.forward);  
            
            // если игрок вне конуса (чекаем радиус конуса + его угол)
            // и не находиться в радиусе inner detection (маленькая область вокруг врага) 
            if ((!(angleToPlayer < _detectionAngle / 2f) || !(dirToPlayer.magnitude < _detectionRadius))
                && !(dirToPlayer.magnitude < _innerDetectionRadius))
            {
                return false;
            }
            
            timer.Start();
            return true;
        }
    }
}
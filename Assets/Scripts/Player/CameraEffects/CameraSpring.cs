using UnityEngine;

namespace Player.CameraEffects
{
    /// <summary>
    /// Добавляем камере инерцию, эффект пружины
    /// </summary>
    public class CameraSpring : MonoBehaviour
    {
        [Header("Animation params")]
        [Min(0.01f)] [SerializeField] private float halfLife = 0.075f;
        [SerializeField] private float frequency = 18f;
        [Header("Camera params")]
        [SerializeField] private float angularDisplacement = 2f;
        [SerializeField] private float linearDisplacement = 0.05f;
        
        // настройки лучше не выводить в редактор - они очень тонкие
        private Vector3 _springPosition;
        private Vector3 _springVelocity;
        
        public void Init()
        {
            _springPosition = transform.position;
            _springVelocity = Vector3.zero;
        }

        public void UpdateSpring(float deltaTime, Vector3 up)
        {
            transform.localPosition = Vector3.zero;
            
            // этим только меняем позицию камеры
            Spring(ref _springPosition, ref _springVelocity, transform.position, halfLife, frequency, deltaTime);

            // тут ее поворачиваем куда нужно
            var localSpringPosition = _springPosition - transform.position;
            var springHeight = Vector3.Dot(localSpringPosition, up);

            transform.localEulerAngles = new Vector3(-springHeight * angularDisplacement, 0f, 0f);
            transform.localPosition = localSpringPosition * linearDisplacement;  // камера находиться вне капсулы? или нет?
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _springPosition);
            Gizmos.DrawSphere(_springPosition, 0.1f);
        }

        // автор спиздил функцию
        // halfTime и frequency отвечают за анимацию
        private static void Spring(ref Vector3 current, ref Vector3 velocity, Vector3 target, float halfTime,
            float frequency, float timeStep)
        {
            var dampingRation = -Mathf.Log(0.5f) / (frequency * halfTime);
            var f = 1f + 2f * timeStep * dampingRation * frequency;
            var oo = frequency * frequency;
            var hoo = timeStep * oo;
            var hhoo = timeStep * hoo;
            var detInv = 1f / (f + hhoo);
            var detX = f * current + timeStep * velocity + hhoo * target;
            var detV = velocity + hoo * (target - current);
            current = detX * detInv;
            velocity = detV * detInv;
        }
    }
}
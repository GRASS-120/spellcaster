using UnityEngine;

namespace Player.CameraEffects
{
    /// <summary>
    /// Наклоняет камеру при движении строго по вертикали или горизонтали (вперед - назад, влево - вправо)
    /// </summary>
    public class CameraLean : MonoBehaviour
    {
        [Header("Camera lean params")]
        [SerializeField] private float attackDamping = 0.5f;
        [SerializeField] private float decayDamping = 0.3f;
        [SerializeField] private float walkStrength = 0.075f;
        [SerializeField] private float slideStrength = 0.2f;
        [SerializeField] private float strengthResponse = 5f;

        private Vector3 _dampedAcceleration;
        private Vector3 _dampedAccelerationVelocity;
        private float _smoothStrength;
        
        public void Init()
        {
            _smoothStrength = walkStrength;
        }

        public void UpdateLean(float deltaTime, bool sliding, Vector3 acceleration, Vector3 up)
        {
            var planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);
            var damping = planarAcceleration.magnitude > _dampedAcceleration.magnitude ? attackDamping : decayDamping;

            _dampedAcceleration = Vector3.SmoothDamp(_dampedAcceleration, planarAcceleration,  // + плавность
                ref _dampedAccelerationVelocity, damping, float.PositiveInfinity, deltaTime);
            
            // get the rotation axis based on the acceleration vector
            var leanAxis = Vector3.Cross(_dampedAcceleration.normalized, up).normalized;
            
            // reset the rotation to that of its parent
            transform.localRotation = Quaternion.identity;
            
            // rotate around lean axis
            var targetStrength = sliding ? slideStrength : walkStrength;
            
            _smoothStrength = Mathf.Lerp(_smoothStrength, targetStrength,
                1f - Mathf.Exp(-strengthResponse * deltaTime));
            
            transform.rotation = Quaternion.AngleAxis(-_dampedAcceleration.magnitude * _smoothStrength, leanAxis) * transform.rotation;

            //Debug.DrawRay(transform.position, acceleration, Color.red);
            //Debug.DrawRay(transform.position, _dampedAcceleration, Color.blue);
        }
    }
}
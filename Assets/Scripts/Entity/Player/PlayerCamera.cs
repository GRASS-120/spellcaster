using Entity.Player.CameraEffects;
using Player;
using UnityEngine;

namespace Entity.Player
{
    public struct CameraInput
    {
        public Vector2 Look;
    }
    
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Camera effects")]
        [SerializeField] private CameraSpring cameraSpring;
        [SerializeField] private CameraLean cameraLean;
        [SerializeField] private CameraSprint cameraSprint;
        [Header("Params")]
        [Range(0f, 1f)][SerializeField] private float sensitivity = 0.1f;
        
        // выделяем в отдельную переменную, так как eulerAngles в Quaternions ведет себя непредсказуемо***
        private Vector3 _eulerAngles;  
        
        public void Init(Transform target)
        {
            transform.position = target.position;
            transform.eulerAngles = _eulerAngles = target.eulerAngles;
            
            cameraSpring.Init();
            cameraLean.Init();
            cameraSprint.Init();
        }

        public void UpdateEffects(Transform target, PlayerControllerState state, float deltaTime)
        {
            cameraSpring.UpdateSpring(deltaTime, target.up);
            cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide, state.Acceleration, target.up);
            cameraSprint.UpdateSprintEffect(deltaTime, state.Stance is Stance.Sprint);
        }

        public void UpdatePosition(Transform target)
        {
            transform.position = target.position;
        }

        public void UpdateRotation(CameraInput input)
        {
            _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
            _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -85f, 85f);  // ограничение на угол обзора по вертикали
            transform.eulerAngles = _eulerAngles;
        }
    }
}

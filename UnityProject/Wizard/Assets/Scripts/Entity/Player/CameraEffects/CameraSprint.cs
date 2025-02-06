using UnityEngine;

namespace Entity.Player.CameraEffects
{
    public class CameraSprint : MonoBehaviour
    {
        [Header("FOV params")]
        [SerializeField] private float defaultFOV = 60f;
        [SerializeField] private float sprintFOV = 80f;
        [SerializeField] private float strengthResponse = 0.075f;
        
        private Camera _mainCamera;
        private float _currentFOV;
        private float _lastFOV;

        public void Init()
        {
            _mainCamera = Camera.main;
            _currentFOV = defaultFOV;
            _lastFOV = defaultFOV;
            _mainCamera.fieldOfView = _currentFOV;
        }
        
        // нельяз переключать спринт в crouch
        // после crouch игрок должен walk
        

        public void UpdateSprintEffect(float deltaTime, bool isSprinting)
        {
            _lastFOV = _currentFOV;
            _currentFOV = isSprinting ? sprintFOV : defaultFOV;
            
            if ((int)_currentFOV == (int)_lastFOV) return;
            
            _currentFOV = Mathf.Lerp(_lastFOV, _currentFOV, 1f - Mathf.Exp(-strengthResponse * deltaTime));
            _mainCamera.fieldOfView = _currentFOV;
        }
    }
}
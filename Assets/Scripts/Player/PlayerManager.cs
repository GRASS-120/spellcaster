using System;
using Player.CameraEffects;
using Player.Interaction;
using Player.VFX;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using PlayerInputManager = Player.Input.PlayerInputManager;

// TODO:
// chromatic aberration

namespace Player
{
    /// <summary>
    /// Связывает PlayerCharacter, PlayerInputManager и PlayerCamera. То есть, получаем команду от игрока
    /// после нажатия на кнопку => формируем запрос с измененными данными => меняються значения в камере и в персонаже
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [Header("Entities")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private PlayerInputManager input;
        [Header("VFX")]
        [SerializeField] private Volume volume;
        [SerializeField] private StanceVignette stanceVignette;

        public PlayerInputManager Input => input;
        public Collider PlayerCollider => playerController.GetComponent<Collider>();
        public PlayerCamera Camera => playerCamera;

        private PlayerInteractor _interactor;

        private void Awake()
        {
            _interactor = GetComponent<PlayerInteractor>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            playerController.Init();
            playerCamera.Init(playerController.CameraTarget);
            
            _interactor.Init();

            stanceVignette.Init(volume.profile);
        }

        private void Update()
        {
            var cameraInput = new CameraInput {Look = input.LookDir};
            var deltaTime = Time.deltaTime;
            
            playerCamera.UpdateRotation(cameraInput);

            // нет смысла переделывать, будет гораздо больеше кода, и это не даст никаких преимуществ при этом.
            // нас интересует не то, когда изменяеться состояние, а то, какое значение у него => от ивентов смысла мало
            var characterInput = new CharacterInput
            {
                Rotation = playerCamera.transform.rotation,
                Move = input.Dir,
                Sprint = input.Sprint ? SprintInput.Toggle : SprintInput.None,
                Jump = input.Jump,
                JumpSustain = input.JumpSustain,
                Crouch = input.Crouch ? CrouchInput.Toggle : CrouchInput.None
            };
            
            playerController.HandleInput(characterInput);
            playerController.UpdateBody(deltaTime);
            
            #if UNITY_EDITOR
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                if (Physics.Raycast(ray, out var hit))
                {
                    Teleport(hit.point);
                }
            }
            #endif
        }

        // изменять камеру лучше в LateUpdate
        private void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            var cameraTarget = playerController.CameraTarget;
            var state = playerController.State;
            
            playerCamera.UpdatePosition(cameraTarget);
            playerCamera.UpdateEffects(cameraTarget, state, deltaTime);
            // cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
            // cameraLean.UpdateLean(deltaTime, state.Stance is Stance.Slide, state.Acceleration, cameraTarget.up);
            // cameraSprint.UpdateSprintEffect(deltaTime, state.Stance is Stance.Sprint);
            
            stanceVignette.UpdateVignette(deltaTime, state.Stance);
        }

        public void Teleport(Vector3 position)
        {
            playerController.SetPosition(position);
        }
    }
}

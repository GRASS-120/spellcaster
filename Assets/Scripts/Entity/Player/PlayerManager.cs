using System.ComponentModel.DataAnnotations;
using Entity.Player.Interaction;
using Entity.Player.VFX;
using Interactable;
using StatsManager;
using StatsManager.StatsTypes;
using UnityEngine;
using UnityEngine.Rendering;
using PlayerInputManager = Entity.Player.Input.PlayerInputManager;

// TODO:
// chromatic aberration

namespace Entity.Player
{
    /// <summary>
    /// Связывает PlayerCharacter, PlayerInputManager и PlayerCamera. То есть, получаем команду от игрока
    /// после нажатия на кнопку => формируем запрос с измененными данными => меняються значения в камере и в персонаже
    /// </summary>
    public class PlayerManager : MonoBehaviour, IEntity, IItemVisitable
    {
        // TODO: LOCAL INSTANCE OF HP AND MANA! 
        
        [Header("Entities")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private PlayerInputManager input;
        [SerializeField, Required] private BaseStats baseStats;
        [Header("VFX")]
        [SerializeField] private Volume volume;
        [SerializeField] private StanceVignette stanceVignette;

        public Stats Stats { get; set; }

        public PlayerInputManager Input => input;
        public PlayerController PlayerController => playerController;
        public Stance Stance => playerController.State.Stance;
        public PlayerCamera Camera => playerCamera;
        public PlayerInteractor Interactor => _interactor;

        private PlayerInteractor _interactor;

        private void Awake()
        {
            _interactor = GetComponent<PlayerInteractor>();
            // base.Awake();
            Stats = new Stats(new StatsMediator(), baseStats);
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            playerController.Init(this);
            playerCamera.Init(playerController.CameraTarget);
            
            _interactor.Init();

            stanceVignette.Init(volume.profile);
        }

        private void Update()
        {
            Stats.Mediator.Update(Time.deltaTime);

            var cameraInput = new CameraInput {Look = input.LookDir};
            
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
            playerController.UpdateBody(Time.deltaTime);
            
            Debug.Log(Stats);
        }

        // изменять камеру лучше в LateUpdate
        private void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            var cameraTarget = playerController.CameraTarget;
            var state = playerController.State;
            
            playerCamera.UpdatePosition(cameraTarget);
            playerCamera.UpdateEffects(cameraTarget, state, deltaTime);

            stanceVignette.UpdateVignette(deltaTime, state.Stance);
        }

        public void Accept(IItemVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

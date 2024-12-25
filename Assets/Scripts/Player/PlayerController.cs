using KinematicCharacterController;
using UnityEngine;

// !!! ну тут прям напрашиваеться STATE MACHINE со своими переходами, состояниями и тп
// переделать на r3 и ивенты? таймеры те же... + как мне кажется очень много лишних переменных

// !!! нужно исправить, что нельзя присесть в воздухе

// TODO: когда я разберу математику получше, нужно вернуться сюда и опять разобрать код
// TODO: как будто нужно больше декларативности в коде... много параметров слишком, запутаться легко
// TODO: дудумаю нужно переделать состояния... много or + ?. как будто можно упростить и сделать подекларативней

// ???????? ВЫДЕЛИТЬ SPRINT В STANCE ИЛИ В ОТДЕЛЬНЫЙ BOOL?

namespace Player
{
    public struct CharacterInput
    {
        public Quaternion Rotation;
        public Vector2 Move;
        public SprintInput Sprint;
        public bool Jump;
        public bool JumpSustain; // в некоторых играх чем сильнее зажимаешь кнопку прыжка, тем выше прыгаешь - вот это оно
        public CrouchInput Crouch;
    }

    public enum CrouchInput
    {
        None, Toggle
    }
    
    public enum SprintInput
    {
        None, Toggle
    }

    public enum Stance
    {
        Walk, Sprint, Crouch, Slide,   // falling?
    }

    public struct PlayerControllerState
    {
        public bool Grounded;
        public Stance Stance;
        public Vector3 Velocity;
        public Vector3 Acceleration;
    }
    
    public class PlayerController : MonoBehaviour, ICharacterController
    {
        [Header("Entities")]
        [SerializeField] private KinematicCharacterMotor motor;
        [SerializeField] private Transform root;
        [SerializeField] private Transform cameraTarget;
        
        [Header("Movement params")]
        [SerializeField] private float moveSpeed = 13f;
        [SerializeField] private float sprintSpeed = 20f;
        [SerializeField] private float crouchSpeed = 7f;
        
        [Header("Movement animation params")]
        [SerializeField] private float moveResponse = 25f;
        [SerializeField] private float crouchResponse = 20f;
        
        [Header("Jump and Gravity params")] 
        [SerializeField] private float jumpSpeed = 20f;
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private float gravity = -90f;
        [Range(0f, 1f)] [SerializeField] private float jumpSustainGravityMult = 0.4f;

        [Header("Sliding params")] 
        [SerializeField] private float slideStartSpeed = 25f;
        [SerializeField] private float slideEndSpeed = 15f;
        [SerializeField] private float slideFriction = 0.8f;
        [SerializeField] private float slideSteerAcceleration = 5f;  // 5f
        [SerializeField] private float slideGravity = -90f;  // выделяем в отедльную переменную, чтобы не зависело от обычной гравитации
        
        [Header("In Air control params")]
        [SerializeField] private float airSpeed = 15f;
        [SerializeField] private float airAcceleration = 200f;
        
        [Header("Crouch params")]
        [SerializeField] private float standHeight = 1.8f;
        [SerializeField] private float crouchHeight = 1.2f;
        [SerializeField] private float crouchHeightResponse = 15f;
        [Range(0f, 1f)][SerializeField] private float standCameraTargetHeight = 0.9f;
        [Range(0f, 1f)][SerializeField] private float crouchCameraTargetHeight = 0.7f;

        private PlayerControllerState _state;
        private PlayerControllerState _lastState;  // для того, чтобы знать какое состояние было перед тем как менять
        private PlayerControllerState _tempState;  // временная замена _lastState... (оно нужно, чтобы UpdateVelocity и rotation успевали чекать  _lastState....)

        public PlayerControllerState State => _state;
        private PlayerControllerState LastState => _lastState;
        
        private Quaternion _requestedRotation;
        private Vector3 _requestedMovement;
        private bool _requestedSprint;
        private bool _requestedJump;
        private bool _requestedJumpSustain;
        private bool _requestedCrouch;
        private bool _requestedCrouchInAir;

        // remake? unitask, corutine, r3...
        private float _timeSinceUngrounded;
        private float _timeSinceJumpRequest;
        private bool _ungroundedDueToJump;

        private Collider[] _uncrouchOverlapResult;

        public Transform CameraTarget => cameraTarget;
        
        public void Init()
        {
            _state.Stance = Stance.Walk;
            _lastState = _state;
            _uncrouchOverlapResult = new Collider[8];
            motor.CharacterController = this;
        }
        
        public void HandleInput(CharacterInput input)
        {
            _requestedRotation = input.Rotation;
            _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
            
            // по диагонали длинее вектор => больше скорость. поэтому везде делаем до 1f
            _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);  
            
            // orient the input => it is relative to the dir the player is facing
            _requestedMovement = input.Rotation * _requestedMovement;

            var wasRequestingJump = _requestedJump;
            _requestedJump = _requestedJump || input.Jump;
            _requestedJumpSustain = input.JumpSustain;
            if (_requestedJump && !wasRequestingJump)
            {
                _timeSinceJumpRequest = 0f;  // начинаем отсчет для coyote time
            }

            var wasRequestingSprint = _requestedSprint;
            _requestedSprint = input.Sprint switch  // + декларативность? переделать? а то неудобно так для всего писать
            {
                SprintInput.Toggle => !_requestedSprint,
                SprintInput.None => _requestedSprint,
                _ => _requestedSprint
            };
            
            // нужно сделать так чтобы прыжок после бега был дальше! - и так есть, но не сильна разница
            // убрать подкат если walk -> crouch
            
            // спринт нельзя в воздухе + спринт должен сохраняться после приземления
            if (_requestedSprint && !wasRequestingSprint && !_state.Grounded)
            {
                _requestedSprint = false;
            }

            var wasRequestingCrouch = _requestedCrouch;
            _requestedCrouch = input.Crouch switch
            {
                CrouchInput.Toggle => !_requestedCrouch,
                CrouchInput.None => _requestedCrouch,
                _ => _requestedCrouch
            };

            // по сути запрещаем слайдить в случае, если не нажали crouch
            if (_requestedCrouch && !wasRequestingCrouch)
            {
                _requestedCrouchInAir = !_state.Grounded;
            } else if (!_requestedCrouch && wasRequestingCrouch)
            {
                _requestedCrouchInAir = false;
            }
        }

        // TODO: когда добавлю модельку, сделать так, чтобы менялся хитбокс? + не нужно скукоживать, просто моделька приседает
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateBody(float deltaTime)
        {
            var currentHeight = motor.Capsule.height;
            var normalizedHeight = currentHeight / standHeight;  // ! это я так понимаю только пока капсула есть. для перса не нужно
            
            var cameraTargetHeight = currentHeight *
                                    (_state.Stance is Stance.Walk or Stance.Sprint ? standCameraTargetHeight : crouchCameraTargetHeight);
            var rootTargetScale = new Vector3(1f, normalizedHeight, 1f);
            
            // анимируем перемещение. 1f - Mathf.Exp(-crouchHeightResponse * deltaTime) - сам автор не понял толком, но задача - 
            // сделать анимацию не сильно зависимой от фрейм рейта (если просто crouchHeightResponse, то больше зависимости типо...)
            cameraTarget.localPosition = Vector3.Lerp(  // ! выделить в отдельнукю функцию
                cameraTarget.localPosition,
                new Vector3(0f, cameraTargetHeight, 0f),
                1f - Mathf.Exp(-crouchHeightResponse * deltaTime));
            
            root.localScale = Vector3.Lerp(
                root.localScale,
                rootTargetScale,
                1f - Mathf.Exp(-crouchHeightResponse * deltaTime));
        }
        
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            // нужно обновлять rotation у PlayerCharacter в то же время что и _requestedRotation (camera rotation)
            // + нужно, чтобы вместе с камерой не наклонялся игрок. для этого нужно сделать проекцию на плоскость в направлении 
            // взгляда -> игрок как бы смотрит вдоль плоскости*** 
            currentRotation = _requestedRotation;

            var forward = Vector3.ProjectOnPlane(_requestedRotation * Vector3.forward, motor.CharacterUp);

            if (forward != Vector3.zero)
            {
                currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);  // ! motor.CharacterUp
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            _state.Acceleration = Vector3.zero;
            
            // --- Stay on ground ---
            if (motor.GroundingStatus.IsStableOnGround)
            {
                _timeSinceUngrounded = 0f;
                _ungroundedDueToJump = false;
                
                // GetDirectionTangentToSurface returns a unit vector => multiply the value by the magnitude to achieve the correct speed
                // GetDirectionTangentToSurface - проверяет направление движения относительно поверхности, используя при этом тангенс угла
                // между направлением и нормалью поверхности (чтобы понять, игок идет по ровной поверхности или по наклонной плоскости и тп)
                var groundedMovement = motor.GetDirectionTangentToSurface(
                    direction: _requestedMovement,
                    surfaceNormal: motor.GroundingStatus.GroundNormal) * _requestedMovement.magnitude;
                
                // --- Start sliding ---
                var moving = groundedMovement.sqrMagnitude > 0f;
                var crouching = _state.Stance is Stance.Crouch;
                var wasStanding = _lastState.Stance is Stance.Sprint;  // ! ВИДИМО ТУТ СЛАЙДИНГ В ЦЕЛОМ => НУЖНО ИЗ SPRINT, КОГДА ПОДКАТ !
                var wasInAir = !_lastState.Grounded;  // даже crouch не считаеться???
                
                if (!moving) _requestedSprint = false;  // если игрок стоит, то спринт не работает

                if (moving && crouching && (wasStanding || wasInAir))  // если я в приседе, то я могу слайдить
                {
                    _state.Stance = Stance.Slide;

                    
                    // TODO: добавляет слайдинг после падения. нужно добавить возможность выключить
                    ///////////
                    // when landing on stable ground the character motor projects the velocity onto a flat ground plane.
                    // see: KinematicCharacterMotor.HandleVelocityProjection()
                    // this is normally good, because under normal circumstance the player shouldn't slide when landing on the ground
                    // in this case (пока я смотрю гайд кароче) we want the player to slide.
                    // reproject the last frames (failing) velocity onto the ground normal to slide
                    if (wasInAir)
                    {
                        currentVelocity =
                            Vector3.ProjectOnPlane(_lastState.Velocity, motor.GroundingStatus.GroundNormal);
                    }
                    ///////////
                    
                    // crouch in air check - можем приседать в воздухе. исправить это?
                    var effectiveSlideStartSpeed = slideStartSpeed;
                    if (!_lastState.Grounded && !_requestedCrouchInAir)
                    {
                        effectiveSlideStartSpeed = 0f;
                        _requestedCrouchInAir = false;
                    }

                    var slideSpeed = Mathf.Max(effectiveSlideStartSpeed, currentVelocity.magnitude);  // не хотим замедлять
                    currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal)
                                      * slideSpeed;
                }

                // --- Move ---
                if (_state.Stance is Stance.Walk or Stance.Crouch or Stance.Sprint)
                {
                    if (_requestedSprint && !_requestedCrouch) _state.Stance = Stance.Sprint;
                    
                    var efficientSpeed = _requestedSprint ? sprintSpeed : moveSpeed;
                    var speed = _state.Stance is Stance.Walk or Stance.Sprint ? efficientSpeed : crouchSpeed;
                    var response = _state.Stance is Stance.Walk or Stance.Sprint ? moveResponse : crouchResponse;

                    // + плавность (targetVelocity - скорость, которую мы хотим набрать по сути)
                    var targetVelocity = groundedMovement * speed;
                    var moveVelocity = Vector3.Lerp(
                        currentVelocity, 
                        targetVelocity, 
                        1f - Mathf.Exp(-response * deltaTime));
                    _state.Acceleration = moveVelocity - currentVelocity;
                    
                    currentVelocity = moveVelocity;
                }
                
                // --- Continue sliding ---
                else
                {
                    // --- Friction ---
                    currentVelocity -= currentVelocity * (slideFriction * deltaTime);
                    
                    // --- Slope ---
                    // получается так, что при спуске буст к скорости, при ровной поверхности буста нет, а при подъеме - замедление
                    var slopeForce = Vector3.ProjectOnPlane(-motor.CharacterUp, motor.GroundingStatus.GroundNormal) *
                                slideGravity;
                    currentVelocity -= slopeForce * deltaTime; 
                    
                    // --- Steer ---
                    // дает контроль при слайде. при этом не ускоряет игрока от поворотов
                    var currentSpeed = currentVelocity.magnitude;  // player movement dir at the current speed
                    var targetVelocity = groundedMovement * currentSpeed;
                    var steerVelocity = currentVelocity;
                    var steerForce = (targetVelocity - steerVelocity) * slideSteerAcceleration * deltaTime;
                    // add steer force, but clamp velocity so the slide does not increase due to direct movement input
                    
                    steerVelocity += steerForce;
                    steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);

                    _state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;

                    currentVelocity = steerVelocity;
                    
                    // если скорость при слайде ниже минимума, то перестаем слайдить и переходим в состояние crouch
                    if (currentVelocity.magnitude < slideEndSpeed)  
                    {
                        _state.Stance = Stance.Crouch;
                    }
                }
            }
            
            // --- In the air ---
            else
            {
                _timeSinceUngrounded += deltaTime;
                
                // --- Move ---
                if (_requestedMovement.sqrMagnitude > 0f)  // if try to move
                {
                    // задумка в том, что игрок по сути должен ходить так же, как и по земле (по XZ), то есть по плоскости => имитируем плоскость
                    // + независимость от vertical velocity
                    
                    // projection on movement plane (direction?)
                    var planarMovement = Vector3.ProjectOnPlane(_requestedMovement, motor.CharacterUp)
                                         * _requestedMovement.magnitude;
                    //planarMovement.Normalize(); // ?????? зачем
                    
                    // current velocity on movement plane
                    var currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
                    
                    var movementForce = planarMovement * airAcceleration * deltaTime;

                    // if moving slower than air speed, treat 
                    if (currentPlanarVelocity.magnitude < airSpeed)
                    {
                        // add it to the 
                        var targetPlanarVelocity = currentPlanarVelocity + movementForce;
                        // limit target velocity to air speed
                        targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);
                        // steer toward current velocity
                        movementForce += targetPlanarVelocity - currentPlanarVelocity;  // для плавности смены скоростей при изменении направления движения
                    }                    
                    
                    /////// ???????????????????????? не понял это момент (фиксит то, что после усокрения из подката инерция не передавалась прыжку)
                    // otherwise, nerf the movement force when it is in the dir of the current
                    // player velocity to prevent accelerating further beyond the max air speed
                    else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)  // проверяем что сила и направление скорости в одном направлении?
                    {
                        // project movement force onto the plane whose normal is the current player velocity
                        var constrainedMovementForce =
                            Vector3.ProjectOnPlane(movementForce, currentPlanarVelocity.normalized);
                        movementForce = constrainedMovementForce;
                    }
                    
                    // prevent air-climbing steep slopes (when jump against the wall and slide on this wall) - отменяем ускорение при моприкосновении со стенами
                    if (motor.GroundingStatus.FoundAnyGround)
                    {
                        // if moving in the same dir as the resultant velocity...
                        if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                        {
                            // ? не понимаю
                            var obstructionNormal = Vector3.Cross(motor.CharacterUp,  // еще не разбирал cross-product (нужен для нахождения нормали к поверхности)
                                Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal)).normalized;  // так вот же нормаль...
                            
                            // project movement force onto obstruction plane
                            movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                        }
                    }
                    
                    currentVelocity += movementForce;
                }
                
                // gravity, because player in the air
                var effectiveGravity = gravity;
                // рисунок. суть - пока игрок набирает высоту, значит работает jumpSustainGravity
                // когда начинает падать, то включаеться обычная гравитация (чтобы игрок падал всегда с обычной скоростью)
                var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);  
                
                if (_requestedJumpSustain && verticalSpeed > 0f)
                {
                    effectiveGravity *= jumpSustainGravityMult;  // по сути удлиняем прыжок за счет снижения гравитации
                }
                
                currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
            }

            // --- Jump ---
            if (_requestedJump)
            {
                var grounded = motor.GroundingStatus.IsStableOnGround;
                // _ungroundedDueToJump = false -> значит на земле? чет не пойму... но если этого нет, то будет двойной прыжок
                var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump; // если coyote time не закончился и игрок не оказался в воздухе из-за прыжка

                if (grounded || canCoyoteJump)
                {
                    _requestedJump = false;
                    _requestedCrouch = false;  // после прыжка персонаж встает из приседа
                    _requestedCrouchInAir = false;
                
                    // unstick the player from the ground
                    motor.ForceUnground(time: 0f);
                    _ungroundedDueToJump = true;
                    
                    // set min vert. speed for the jump speed
                    var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                    var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                
                    // add the difference in current and target vert. speed
                    currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);  // ?
                }
                else
                {
                    _timeSinceJumpRequest += deltaTime;

                    // defer the jump request until coyote time has passed.
                    var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                    
                    // можно прыгать только если на земле
                    _requestedJump = canJumpLater;
                }
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            _tempState = _state;
            
            // crouch
            if (_requestedCrouch && _state.Stance is Stance.Walk or Stance.Sprint)
            {
                _state.Stance = Stance.Crouch;
                motor.SetCapsuleDimensions  // мб не меняеться
                (
                    radius: motor.Capsule.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                );
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // sliding должен быть только на земле
            if (!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Slide)
            {
                _state.Stance = Stance.Crouch;
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            // Stuff can happen after move acceleration (walk/slide movement) is applied to current velocity that
            // lowers the velocity, so after the character updates make sure move acceleration does not exceed
            // the total acceleration.
            var totalAcceleration = (_state.Velocity - _lastState.Velocity) / deltaTime;
            _state.Acceleration = Vector3.ClampMagnitude(_state.Acceleration, totalAcceleration.magnitude);
            
            // uncrouch - here because of needing extra logic (ceilings)
            if (!_requestedCrouch && _state.Stance is not Stance.Walk or Stance.Sprint)
            {
                // tentatively "standup" the character capsule
                motor.SetCapsuleDimensions   // мб не меняеться
                (
                    radius: motor.Capsule.radius,
                    height: standHeight,
                    yOffset: standHeight * 0.5f
                );
                
                // then see if the capsule overlaps any collider before actually
                // allowing the character to standup
                var pos = motor.TransientPosition;
                var rot = motor.TransientRotation;
                var mask = motor.CollidableLayers;
                
                if (motor.CharacterOverlap(pos, rot, _uncrouchOverlapResult, mask, QueryTriggerInteraction.Ignore) > 0)
                {
                    // если перекрыват другие колладйеры => не может встать => возвращаем коллайдер обратно в crouch
                    _requestedCrouch = true;
                    motor.SetCapsuleDimensions
                    (
                        radius: motor.Capsule.radius,
                        height: crouchHeight,
                        yOffset: crouchHeight * 0.5f
                    );
                }
                else
                {
                    // если игрок выходит из слайда ИЛИ он выходит из приседа, то спринт отменяеться и игрок прост идет
                    if (_lastState.Stance == Stance.Slide || (_lastState.Stance == Stance.Crouch && !_requestedCrouch))
                    {
                        _requestedSprint = false;
                    }

                    // возвращаем игрока в состояние Walk или Sprint, если игрок не в приседе или не слайдит
                    _state.Stance = _requestedSprint ? Stance.Sprint : Stance.Walk;
                }
            }

            _state.Grounded = motor.GroundingStatus.IsStableOnGround;  // sync with character motor ground status
            _state.Velocity = motor.Velocity;
            _lastState = _tempState;
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            _state.Acceleration = Vector3.ProjectOnPlane(_state.Acceleration, hitNormal);
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        public void SetPosition(Vector3 position, bool killVelocity = true)
        {
            motor.SetPosition(position);
            if (killVelocity) motor.BaseVelocity = Vector3.zero;
        }
    }
}

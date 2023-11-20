// Designed by KINEMATION, 2023

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.FPSAnimator;
using Kinemation.FPSFramework.Runtime.Layers;
using Kinemation.FPSFramework.Runtime.Recoil;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Fusion;
using Demo.Scripts.Runtime;

namespace InGame.Player
{
    public class NetworkFPSController : FPSAnimController
    {
        [Header("General")]
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private PlayerRPCManager _playerManager;
        [SerializeField]
        private PlayerStatus _playerStatus;

        [SerializeField] private float turnInPlaceAngle;
        [SerializeField] private AnimationCurve turnCurve = new AnimationCurve(new Keyframe(0f, 0f));
        [SerializeField] private float turnSpeed = 1f;

        [Header("Dynamic Motions")]
        [SerializeField] private IKAnimation aimMotionAsset;
        [SerializeField] private IKAnimation leanMotionAsset;
        [SerializeField] private IKAnimation crouchMotionAsset;
        [SerializeField] private IKAnimation unCrouchMotionAsset;
        [SerializeField] private IKAnimation onJumpMotionAsset;
        [SerializeField] private IKAnimation onLandedMotionAsset;

        // Animation Layers
        [SerializeField] [HideInInspector] private LookLayer lookLayer;
        [SerializeField] [HideInInspector] private AdsLayer adsLayer;
        [SerializeField] [HideInInspector] private SwayLayer swayLayer;
        [SerializeField] [HideInInspector] private LocomotionLayer locoLayer;
        [SerializeField] [HideInInspector] private SlotLayer slotLayer;
        // Animation Layers

        [Header("General")]
        [SerializeField] private CharacterController controller;
        private NetworkCharacterControllerPrototype netWorkcontroller;
        [SerializeField] private float gravity = 8f;
        [SerializeField] private float jumpHeight = 9f;
        private bool _isInAir = false;

        [Header("Camera")]
        [SerializeField] private Transform mainCamera;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Transform firstPersonCamera;
        [SerializeField] private float sensitivity;
        [SerializeField] private Vector2 freeLookAngle;

        [Header("Movement")]
        [SerializeField] private float curveLocomotionSmoothing = 2f;
        [SerializeField] private float moveSmoothing = 2f;
        [SerializeField] private float sprintSpeed = 3f;
        [SerializeField] private float walkingSpeed = 2f;
        [SerializeField] private float crouchSpeed = 1f;
        [SerializeField] private float crouchRatio = 0.5f;
        private float speed;

        [SerializeField] private List<Weapon> weapons;
        [SerializeField] private FPSCameraShake shake;

        private bool disableInput = false;
        private Vector2 _playerInput;
        private bool initialize = false;

        // Used for free-look
        private Vector2 _freeLookInput;
        private Vector2 _smoothAnimatorMove;
        private Vector2 _smoothMove;

        private int _index;
        private int _lastIndex;

        private float _fireTimer = -1f;
        private int _bursts;
        private bool _aiming;
        private bool _freeLook;
        private bool _hasActiveAction;

        public FPSActionState actionState;
        private FPSMovementState movementState;
        private FPSPoseState poseState;
        private FPSCameraState cameraState = FPSCameraState.Default;

        private float originalHeight;
        private Vector3 originalCenter;

        private float smoothCurveAlpha = 0f;

        private static readonly int Crouching = Animator.StringToHash("Crouching");
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int OverlayType = Animator.StringToHash("OverlayType");
        private static readonly int TurnRight = Animator.StringToHash("TurnRight");
        private static readonly int TurnLeft = Animator.StringToHash("TurnLeft");
        private static readonly int InAir = Animator.StringToHash("InAir");
        private static readonly int Equip = Animator.StringToHash("Equip");
        private static readonly int UnEquip = Animator.StringToHash("Unequip");

        private void InitLayers()
        {
            Debug.Log("InitLayers");
            InitAnimController();

            controller = GetComponentInChildren<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            lookLayer = GetComponentInChildren<LookLayer>();
            adsLayer = GetComponentInChildren<AdsLayer>();
            locoLayer = GetComponentInChildren<LocomotionLayer>();
            swayLayer = GetComponentInChildren<SwayLayer>();
            slotLayer = GetComponentInChildren<SlotLayer>();
        }
        private void Awake()
        {
            netWorkcontroller = GetComponent<NetworkCharacterControllerPrototype>();
        }
        private void Start()
        {
            Time.timeScale = 1f;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            speed = walkingSpeed;

            originalHeight = controller.height;
            originalCenter = controller.center;

            moveRotation = transform.rotation;

            InitLayers();
            EquipWeapon();
            initialize = true;
        }

        private void StartWeaponChange()
        {
            DisableAim();

            _hasActiveAction = true;
            _playerManager.CrossFade(UnEquip, 0.1f);
        }

        public void SetActionActive(int isActive)
        {
            _hasActiveAction = isActive != 0;
        }

        public void RefreshStagedState()
        {
            GetGun().stagedReloadSegment++;
        }

        public void ResetStagedState()
        {
            GetGun().stagedReloadSegment = 0;
        }

        public void EquipWeapon()
        {
            if (weapons.Count == 0) return;

            weapons[_lastIndex].gameObject.SetActive(false);
            var gun = weapons[_index];

            _bursts = gun.burstAmount;

            StopAnimation(0.1f);
            InitWeapon(gun);
            gun.gameObject.SetActive(true);

            _playerManager.SetFloat(OverlayType, (float)gun.overlayType);
            _playerManager.Play(Equip);
        }

        private void ChangeWeapon_Internal()
        {
            if (movementState == FPSMovementState.Sprinting) return;
            if (_hasActiveAction) return;

            OnFireReleased();

            int newIndex = _index;
            newIndex++;
            if (newIndex > weapons.Count - 1)
            {
                newIndex = 0;
            }

            _lastIndex = _index;
            _index = newIndex;

            StartWeaponChange();
        }

        private void DisableAim()
        {
            _aiming = false;
            OnInputAim(_aiming);

            _playerManager.SetActionState(FPSActionState.None);
            adsLayer.SetAds(false);
            adsLayer.SetPointAim(false);
            swayLayer.SetFreeAimEnable(true);
            swayLayer.SetLayerAlpha(1f);
            _playerManager.PlayMotionAsset(aimMotionAsset);
        }

        public void ToggleAim()
        {
            if (_hasActiveAction)
            {
                //return;
            }

            _aiming = !_aiming;

            if (_aiming)
            {
                _playerManager.SetActionState(FPSActionState.Aiming);
                adsLayer.SetAds(true);
                swayLayer.SetFreeAimEnable(false);
                swayLayer.SetLayerAlpha(0.3f);
                _playerManager.PlayMotionAsset(aimMotionAsset);
                OnInputAim(_aiming);
            }
            else
            {
                DisableAim();
            }

            recoilComponent.isAiming = _aiming;
        }

        public void ChangeScope()
        {
            InitAimPoint(GetGun());
        }

        public void Fire()
        {
            if (_hasActiveAction) return;

            GetGun().OnFire();
            recoilComponent.Play();
            PlayCameraShake(shake);
        }

        public void OnFirePressed()
        {
            if (weapons.Count == 0) return;
            if (_hasActiveAction) return;

            Fire();
            _bursts = GetGun().burstAmount - 1;
            _fireTimer = 0f;
        }

        private Weapon GetGun()
        {
            return weapons[_index];
        }

        public void OnFireReleased()
        {
            recoilComponent.Stop();
            _fireTimer = -1f;
        }

        private void SprintPressed()
        {
            if (poseState == FPSPoseState.Crouching || _hasActiveAction || _isInAir)
            {
                return;
            }

            OnFireReleased();
            lookLayer.SetLayerAlpha(0.5f);
            adsLayer.SetLayerAlpha(0f);
            locoLayer.SetReadyWeight(0f);

            movementState = FPSMovementState.Sprinting;
            _playerManager.SetActionState(FPSActionState.None);

            recoilComponent.Stop();
            Debug.Log("SPEEDUP");

            speed = sprintSpeed;
        }

        public void SprintReleased()
        {
            if (poseState == FPSPoseState.Crouching)
            {
                return;
            }
            lookLayer.SetLayerAlpha(1f);
            adsLayer.SetLayerAlpha(1f);
            movementState = FPSMovementState.Walking;

            Debug.Log("SPEEDDOWN");
            speed = walkingSpeed;
        }

        private void Crouch()
        {
            //todo: crouching implementation

            float crouchedHeight = originalHeight * crouchRatio;
            float heightDifference = originalHeight - crouchedHeight;

            controller.height = crouchedHeight;

            // Adjust the center position so the bottom of the capsule remains at the same position
            Vector3 crouchedCenter = originalCenter;
            crouchedCenter.y -= heightDifference / 2;
            controller.center = crouchedCenter;

            speed = crouchSpeed;

            lookLayer.SetPelvisWeight(0f);

            poseState = FPSPoseState.Crouching;
            _playerManager.SetBool(Crouching, true);
            _playerManager.PlayMotionAsset(crouchMotionAsset);
        }

        private void Uncrouch()
        {
            //todo: crouching implementation
            controller.height = originalHeight;
            controller.center = originalCenter;

            speed = walkingSpeed;

            lookLayer.SetPelvisWeight(1f);

            poseState = FPSPoseState.Standing;
            _playerManager.SetBool(Crouching, false);
            _playerManager.PlayMotionAsset(crouchMotionAsset);
        }

        private void TryReload()
        {
            if (movementState == FPSMovementState.Sprinting || _hasActiveAction) return;

            var reloadClip = GetGun().reloadClip;

            if (reloadClip == null) return;

            OnFireReleased();
            //DisableAim();

            PlayAnimation(reloadClip);
            GetGun().Reload();
        }

        private void TryGrenadeThrow()
        {
            if (movementState == FPSMovementState.Sprinting || _hasActiveAction) return;

            if (GetGun().grenadeClip == null) return;

            OnFireReleased();
            DisableAim();
            PlayAnimation(GetGun().grenadeClip);
        }

        private void UpdateActionInput()
        {
            smoothCurveAlpha = FPSAnimLib.ExpDecay(smoothCurveAlpha, _aiming ? 0.4f : 1f, 10,
                Time.deltaTime);

            _playerManager.SetLayerWeight(3, smoothCurveAlpha);

            if (Input.GetKeyDown(KeyCode.R))
            {
                TryReload();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                TryGrenadeThrow();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                StopAnimation(0.2f);
            }

            _playerManager.SetLeanDirection(0);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("DASH");
                SprintPressed();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                SprintReleased();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeWeapon_Internal();
            }

            if (movementState == FPSMovementState.Sprinting)
            {
                return;
            }

            if (actionState != FPSActionState.Ready)
            {
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyUp(KeyCode.Q)
                                                || Input.GetKeyDown(KeyCode.E) || Input.GetKeyUp(KeyCode.E))
                {
                    _playerManager.PlayMotionAsset(leanMotionAsset);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    _playerManager.SetLeanDirection(1);
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    _playerManager.SetLeanDirection(-1);
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    ToggleAim();
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    ChangeScope();
                }

                if (Input.GetKeyDown(KeyCode.B) && _aiming)
                {
                    if (actionState == FPSActionState.PointAiming)
                    {
                        adsLayer.SetPointAim(false);
                        _playerManager.SetActionState(FPSActionState.Aiming);
                    }
                    else
                    {
                        adsLayer.SetPointAim(true);
                        _playerManager.SetActionState(FPSActionState.PointAiming);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (poseState == FPSPoseState.Standing)
                {
                    Crouch();
                }
                else
                {
                    Uncrouch();
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (actionState == FPSActionState.Ready)
                {
                    _playerManager.SetActionState(FPSActionState.None);
                    locoLayer.SetReadyWeight(0f);
                    lookLayer.SetLayerAlpha(1f);
                }
                else
                {
                    _playerManager.SetActionState(FPSActionState.Ready);
                    locoLayer.SetReadyWeight(1f);
                    lookLayer.SetLayerAlpha(.5f);
                    OnFireReleased();
                }
            }
        }

        private Quaternion desiredRotation;
        private Quaternion moveRotation;
        private float turnProgress = 1f;
        private bool isTurning = false;

        private void TurnInPlace()
        {
            float turnInput = _playerInput.x;
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            turnInput -= _playerInput.x;

            float sign = Mathf.Sign(_playerInput.x);
            if (Mathf.Abs(_playerInput.x) > turnInPlaceAngle)
            {
                if (!isTurning)
                {
                    turnProgress = 0f;

                    _playerManager.ResetTrigger(TurnRight);
                    _playerManager.ResetTrigger(TurnLeft);

                    _playerManager.SetTrigger(sign > 0f ? TurnRight : TurnLeft);
                }

                isTurning = true;
            }

            transform.rotation *= Quaternion.Euler(0f, turnInput, 0f);

            float lastProgress = turnCurve.Evaluate(turnProgress);
            turnProgress += Time.deltaTime * turnSpeed;
            turnProgress = Mathf.Min(turnProgress, 1f);

            float deltaProgress = turnCurve.Evaluate(turnProgress) - lastProgress;

            _playerInput.x -= sign * turnInPlaceAngle * deltaProgress;

            transform.rotation *= Quaternion.Slerp(Quaternion.identity,
                Quaternion.Euler(0f, sign * turnInPlaceAngle, 0f), deltaProgress);

            if (Mathf.Approximately(turnProgress, 1f) && isTurning)
            {
                isTurning = false;
            }
        }

        private float _jumpState = 0f;

        private void UpdateLookInput()
        {
            _freeLook = Input.GetKey(KeyCode.X);

            float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
            float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;

            if (_freeLook)
            {
                // No input for both controller and animation component. We only want to rotate the camera

                _freeLookInput.x += deltaMouseX;
                _freeLookInput.y += deltaMouseY;

                _freeLookInput.x = Mathf.Clamp(_freeLookInput.x, -freeLookAngle.x, freeLookAngle.x);
                _freeLookInput.y = Mathf.Clamp(_freeLookInput.y, -freeLookAngle.y, freeLookAngle.y);

                return;
            }

            _freeLookInput = FPSAnimLib.ExpDecay(_freeLookInput, Vector2.zero, 15f, Time.deltaTime);

            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;

            _playerInput.y = Mathf.Clamp(_playerInput.y, -90f, 90f);
            moveRotation *= Quaternion.Euler(0f, deltaMouseX, 0f);
            TurnInPlace();

            _jumpState = FPSAnimLib.ExpDecay(_jumpState, _isInAir ? 1f : 0f, 10f, Time.deltaTime);

            float moveWeight = Mathf.Clamp01(Mathf.Abs(_smoothMove.magnitude));
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, moveWeight);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, _jumpState);
            _playerInput.x *= 1f - moveWeight;
            _playerInput.x *= 1f - _jumpState;

            _playerManager.SetAimInput(_playerInput);
            _playerManager.SetAddDeltaInput(new Vector2(deltaMouseX, charAnimData.deltaAimInput.y));
        }

        private void UpdateFiring()
        {
            if (recoilComponent == null) return;

            if (recoilComponent.fireMode != FireMode.Semi && _fireTimer >= 60f / GetGun().fireRate)
            {
                Fire();

                if (recoilComponent.fireMode == FireMode.Burst)
                {
                    _bursts--;

                    if (_bursts == 0)
                    {
                        _fireTimer = -1f;
                        OnFireReleased();
                    }
                    else
                    {
                        _fireTimer = 0f;
                    }
                }
                else
                {
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer >= 0f)
            {
                _fireTimer += Time.deltaTime;
            }
        }

        private bool IsZero(float value)
        {
            return Mathf.Approximately(0f, value);
        }

        private float verticalVelocity = 0f;
        private float smoothIsMoving = 0f;

        private void UpdateMovement()
        {
            Vector3 velocity = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                velocity += transform.forward;

            if (Input.GetKey(KeyCode.S))
                velocity -= transform.forward;

            if (Input.GetKey(KeyCode.A))
                velocity -= transform.right;

            if (Input.GetKey(KeyCode.D))
                velocity += transform.right;

            velocity = velocity.normalized;
            Vector3 nonVelocity = velocity;

            _playerManager.SetMoveInput(new Vector2(velocity.x, velocity.y));

            velocity *= 1f - _jumpState;

            Vector2 rawInput = new Vector2(velocity.x, velocity.y);
            Vector2 normInput = new Vector2(velocity.x, velocity.y);
            normInput.Normalize();

            if (velocity.Equals(Vector3.zero)
                && movementState == FPSMovementState.Sprinting)
            {
                SprintReleased();
            }

            if (movementState == FPSMovementState.Sprinting)
            {
                normInput.x = rawInput.x = 0f;
                normInput.y = rawInput.y = 2f;
            }

            _smoothMove = FPSAnimLib.ExpDecay(_smoothMove, normInput, moveSmoothing, Time.deltaTime);
            _smoothMove = nonVelocity;//ï‚ê≥Ç»ÇµÇÃà⁄ìÆä÷êî
            velocity.x = _smoothMove.x;
            velocity.y = _smoothMove.y;

            _playerManager.SetMoveInput(normInput);

            _smoothAnimatorMove.x = FPSAnimLib.ExpDecay(_smoothAnimatorMove.x, rawInput.x, 5f, Time.deltaTime);
            _smoothAnimatorMove.y = FPSAnimLib.ExpDecay(_smoothAnimatorMove.y, rawInput.y, 4f, Time.deltaTime);

            bool idle = Mathf.Approximately(0f, normInput.magnitude);
            _playerManager.SetBool(Moving, !idle);

            smoothIsMoving = FPSAnimLib.ExpDecay(smoothIsMoving, idle ? 0f : 1f, curveLocomotionSmoothing,
                Time.deltaTime);

            _playerManager.SetFloat(Velocity, Mathf.Clamp01(smoothIsMoving));
            _playerManager.SetFloat(MoveX, _smoothAnimatorMove.x);
            _playerManager.SetFloat(MoveY, _smoothAnimatorMove.y);

            if (_isInAir)
            {
                verticalVelocity -= gravity * Time.deltaTime;
                verticalVelocity = Mathf.Max(-30f, verticalVelocity);
            }
            velocity = velocity * speed;
            velocity.y = verticalVelocity;
            Debug.Log($"velocity = {velocity}");
            netWorkcontroller.Move(velocity);

            bool bWasInAir = _isInAir;
            _isInAir = !controller.isGrounded;
            _playerManager.SetBool(InAir, _isInAir);

            if (!_isInAir)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    verticalVelocity = jumpHeight;
                }
            }

            if (bWasInAir != _isInAir)
            {
                if (_isInAir)
                {
                    SprintReleased();
                }
                else
                {
                    verticalVelocity = -0.5f;
                }

                _playerManager.PlayMotionAsset(_isInAir ? onJumpMotionAsset : onLandedMotionAsset);
            }
        }
        public void setCharAnimData(Vector2 vector2)
        {
            charAnimData.moveInput = vector2;
        }


        public override void FixedUpdateNetwork()
        {
            if (!_playerStatus.isLocalPlayer) return;
            if (initialize)
            {
                if (!disableInput)
                {
                    UpdateMovement();
                }

                UpdateAnimController();
            }
        }

        private void Update()
        {
            if (!_playerStatus.isLocalPlayer) return;
            if (!disableInput && initialize)
            {
                UpdateActionInput();
                UpdateFiring();
                UpdateLookInput();
            }
        }

        private Quaternion _smoothBodyCam;

     
        public void UpdateCameraRotation()
        {
            Vector2 finalInput = new Vector2(_playerInput.x, _playerInput.y);
            (Quaternion, Vector3) cameraTransform =
                (transform.rotation * Quaternion.Euler(finalInput.y, finalInput.x, 0f),
                    firstPersonCamera.position);

            cameraHolder.rotation = cameraTransform.Item1;
            cameraHolder.position = cameraTransform.Item2;

            mainCamera.rotation = cameraHolder.rotation * Quaternion.Euler(_freeLookInput.y, _freeLookInput.x, 0f);
        }

        public void PlayMotionAsset(string animationName)
        {
            animationName = animationName.Replace(" (Kinemation.FPSFramework.Runtime.FPSAnimator.IKAnimation)", "");
            IKAnimation animation = null;
            switch (animationName)
            {
                case "IKAnim_Aim":
                    animation = aimMotionAsset;
                    break;
                case "IKAnim_Lean":
                    animation = leanMotionAsset;
                    break;
                case "IKAnim_Crouch":
                    animation = crouchMotionAsset;
                    break;
                case "IKAnim_Uncrouch":
                    animation = unCrouchMotionAsset;
                    break;
                case "IKAnim_Jump":
                    animation = onJumpMotionAsset;
                    break;
                case "IKAnim_StopMoving":
                    animation = onLandedMotionAsset;
                    break;
                default:
                    Debug.LogWarning($"null Motion Asset.string is {animationName}");
                    break;
            }
            slotLayer.PlayMotion(animation);
        }

        public void SetLeanDirection(int value)
        {
            charAnimData.leanDirection = value;
        }
        public void SetMoveInput(Vector2 value)
        {
            charAnimData.moveInput = value;
        }

        public void SetAimInput(Vector2 value)
        {
            charAnimData.SetAimInput(value);
        }

        public void SetAddDeltaInput(Vector2 value)
        {
            charAnimData.AddDeltaInput(value);
        }
    }
}
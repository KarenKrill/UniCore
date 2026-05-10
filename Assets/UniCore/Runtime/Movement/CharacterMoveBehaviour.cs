using KarenKrill.UniCore.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class CharacterMoveBehaviour : MonoBehaviour
    {
        /// <summary>Range: [0..1]</summary>
        public float SpeedModifier { get => _speedModifier; set => _speedModifier = value; }
        /// <summary>Range: [0..1]</summary>
        public float GravityModifier { get => _gravityModifier; set => _gravityModifier = value; }
        public bool IsPulsedUp => TryGetAbility<JumpMovement>(out var jumpAbility) && jumpAbility.IsActive;
        public CameraType CameraType { get => _cameraType; set => _cameraType = value; }

        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
        public Vector2 LookDirection { get => _lookDirection; set => _lookDirection = value; }
        public IList<IMoveAbility> Abilities => _abilities;

        public void PulseUp(float distance, float gracePeriod)
        {
            var gravity = Mathf.Abs(Physics.gravity.y) * _gravityMultiplier * _gravityModifier;
            if (TryGetAbility<JumpMovement>(out var jumpMovement))
            {
                jumpMovement.PulseUp(distance, gracePeriod, gravity);
            }
        }

        protected virtual void Awake()
        {
            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main.transform;
            }
            _characterControllerStepOffset = _characterController.stepOffset;
            _characterController.enabled = false;
        }
        protected virtual void OnEnable()
        {
            _characterController.enabled = true;
        }
        protected virtual void OnDisable()
        {
            if (!_characterController.IsNullOrDestroyed())
            {
                _characterController.enabled = false;
            }
        }
        protected virtual void Update()
        {
            UpdateGroundState(_movementCtx);

            var cameraRelativeQuaternion = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up);
            var moveDirection = cameraRelativeQuaternion * _moveDirection;
            var moveIntensity = moveDirection.magnitude;
            if (moveIntensity > 1)
            {
                moveDirection.Normalize();
                moveIntensity = SpeedModifier;
            }
            else
            {
                moveIntensity *= SpeedModifier;
            }
            if (!_useRootMotion)
            {
                var maxSpeed = _movementCtx.IsGrounded ? _maxSpeed : _maxInAirSpeed;
                float speed = moveIntensity * maxSpeed;
                var inputVelocity = speed * moveDirection;
                _movementCtx.Velocity = new(inputVelocity.x, _verticalSpeed, inputVelocity.z);
            }
            else
            {
                _movementCtx.Velocity = new(0, _verticalSpeed, 0);
            }
            _movementCtx.Position = _characterController.transform.position;

            if (TryGetAbility<SlopeSlideMovement>(out var slopeSlideAbility))
            {
                slopeSlideAbility.Options.MinSlidingSlopeAngle = _characterController.slopeLimit;
                slopeSlideAbility.Options.Gravity = Physics.gravity.y;
            }
            foreach (var ability in _abilities)
            {
                if (ability?.Enabled ?? false)
                {
                    ability.Update(_movementCtx);
                }
            }

            // Check on falling
            if (_movementCtx.IsGroundedCoyote)
            {
                _characterController.stepOffset = _characterControllerStepOffset;
            }
            else
            {
                _characterController.stepOffset = 0; // fix stuck in the wall while jumping
            }

            _characterController.Move(_movementCtx.Velocity * Time.deltaTime);
            if (TryUpdateRotation(cameraRelativeQuaternion, moveDirection, _lookDirection, out var rotation))
            {
                _characterController.transform.rotation = rotation.Value;
            }

            ApplyGravity(_movementCtx);

            UpdateAnimationsIfExists(moveIntensity,
                isMoving: moveDirection != Vector3.zero,
                isLooking: _lookDirection != Vector3.zero,
                isJumping: IsPulsedUp,
                isGrounded: _movementCtx.IsGrounded && _movementCtx.IsGroundStable);
        }

        protected virtual void OnAnimatorMove()
        {
            if (_useRootMotion && _animator != null && _movementCtx.IsGrounded && _movementCtx.IsGroundStable)
            {
                Vector3 velocity = _animator.deltaPosition;
                velocity.y = _verticalSpeed * Time.deltaTime;
                _characterController.Move(velocity);
            }
        }

        private static readonly Lazy<int> IsLookingHash = new(() => Animator.StringToHash("IsLooking"));
        private static readonly Lazy<int> IsMovingHash = new(() => Animator.StringToHash("IsMoving"));
        private static readonly Lazy<int> IsFallingHash = new(() => Animator.StringToHash("IsFalling"));
        private static readonly Lazy<int> IsJumpingHash = new(() => Animator.StringToHash("IsJumping"));
        private static readonly Lazy<int> IsGroundedHash = new(() => Animator.StringToHash("IsGrounded"));
        private static readonly Lazy<int> InputMagnitudeHash = new(() => Animator.StringToHash("InputMagnitude"));

        [SerializeField]
        private CharacterController _characterController;
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private Animator _animator = null;
        [SerializeField]
        private float _maxSpeed = 5f, _maxInAirSpeed = 0.5f;
        /// <summary>Max angular speed in degrees</summary>
        [SerializeField]
        private float _maxAngularSpeed = 360.0f;
        [SerializeField, Min(0)]
        private float _maxFallSpeed = 100;
        [SerializeField, Range(0, 1)]
        private float _speedModifier = 1f;
        [SerializeField, Range(0, 1)]
        private float _gravityModifier = 1f;
        [SerializeField]
        private float _gravityMultiplier = 1.5f;
        [SerializeField]
        private float _coyoteTime = 0.2f;
        [SerializeField]
        private bool _useRootMotion = false;
        /// <summary>
        /// Used to rotate character in look direction
        /// </summary>
        /// <remarks>In firs-person look direction is <see cref="_cameraTransform"/> forward, </remarks>
        [SerializeField]
        private CameraType _cameraType = CameraType.FirstPerson;
        [SerializeReference, SerializeInterface]
        private List<IMoveAbility> _abilities = new();

        private readonly MovementContext _movementCtx = new(Vector3.zero, Vector3.zero, isGroundStable: true);

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _lookDirection = Vector2.zero;
        private float _verticalSpeed;
        private float _characterControllerStepOffset;

        private bool TryGetAbility<T>([NotNullWhen(true)] out T ability) where T : IMoveAbility
        {
            ability = (T)_abilities.FirstOrDefault(ability => ability is T);
            return ability is not null;
        }

        private bool TryUpdateRotation(Quaternion cameraRelativeQuaternion, Vector3 direction, Vector3 lookDirection, [NotNullWhen(true)] out Quaternion? rotation)
        {
            bool isMoving = direction != Vector3.zero;
            bool isLooking = lookDirection != Vector3.zero;
            rotation = null;
            if (_cameraType == CameraType.ThirdPerson)
            {
                if (isMoving)
                {
                    var directionLookRotation = Quaternion.LookRotation(direction, Vector3.up);
                    var characterRotation = _characterController.transform.rotation;
                    rotation = Quaternion.RotateTowards(characterRotation, directionLookRotation, _maxAngularSpeed * Time.deltaTime);
                }
            }
            else if (isMoving || isLooking)
            {
                rotation = Quaternion.RotateTowards(_characterController.transform.rotation, cameraRelativeQuaternion, 360);
            }
            return rotation is not null;
        }

        private void UpdateGroundState(MovementContext moveContext)
        {
            moveContext.IsGrounded = _characterController.isGrounded;
            moveContext.CoyoteTime = _coyoteTime;
        }

        private void ApplyGravity(MovementContext moveCtx)
        {
            _verticalSpeed = moveCtx.Velocity.y;
            if (moveCtx.IsGrounded && moveCtx.IsGroundStable)
            {
                _verticalSpeed = -2f; // to prevent isGrounded false positives
            }
            else if (!moveCtx.IsGrounded)
            {
                float gravityAcceleration = Mathf.Abs(Physics.gravity.y) * _gravityMultiplier * _gravityModifier;
                var deltaSpeed = gravityAcceleration * Time.deltaTime;
                _verticalSpeed -= deltaSpeed;
                _verticalSpeed = Mathf.Clamp(_verticalSpeed, -_maxFallSpeed, _maxFallSpeed);
            }
        }

        private void UpdateAnimationsIfExists(float moveIntensity, bool isMoving, bool isLooking, bool isJumping, bool isGrounded)
        {
            if (_animator != null)
            {
                _animator.SetFloat(InputMagnitudeHash.Value, moveIntensity, 0.5f, Time.deltaTime);
                _animator.SetBool(IsGroundedHash.Value, isGrounded);
                _animator.SetBool(IsFallingHash.Value, !isGrounded);
                _animator.SetBool(IsJumpingHash.Value, isJumping);
                _animator.SetBool(IsMovingHash.Value, isMoving);
                _animator.SetBool(IsLookingHash.Value, isLooking);
            }
        }
    }
}

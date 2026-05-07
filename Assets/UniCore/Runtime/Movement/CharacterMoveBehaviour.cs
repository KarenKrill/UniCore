using KarenKrill.UniCore.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public class CharacterMoveBehaviour : MonoBehaviour
    {
        public float MaximumSpeed { get => _maxSpeed; set => _maxSpeed = value; }
        /// <summary>Range: [0..1]</summary>
        public float SpeedModifier { get => _speedModifier; set => _speedModifier = value; }
        /// <summary>Range: [0..1]</summary>
        public float GravityModifier { get => _gravityModifier; set => _gravityModifier = value; }
        public bool IsGrounded => _characterController.isGrounded;
        public bool IsSliding => _slopeSlideMovement.IsActive;
        public bool IsFalling => _fallSpeed > 0;
        public bool IsPulsedUp => _jumpMovement.IsActive;
        public CameraType CameraType { get => _cameraType; set => _cameraType = value; }

        public bool EnableCharController { get => _characterController.enabled; set => _characterController.enabled = value; }
        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
        public Vector2 LookDirection { get => _lookDirection; set => _lookDirection = value; }

        public void PulseUp(float distance, float gracePeriod, float inAirHorizontalSpeed)
        {
            _maxInAirSpeed = inAirHorizontalSpeed;
            var gravity = Mathf.Abs(Physics.gravity.y) * _gravityMultiplier * _gravityModifier;
            _jumpMovement.PulseUp(distance, gracePeriod, gravity);
        }

        protected virtual void Awake()
        {
            _slopeSlideMovement = new(_slopeSlideOptions);
            _jumpMovement = new();
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
            _movementCtx.IsGrounded = _characterController.isGrounded;
            if (_movementCtx.IsGrounded)
            {
                _lastGroundedTime = Time.time;
            }
            _movementCtx.IsGroundedCoyote = Time.time - _lastGroundedTime <= _coyoteTime;

            var cameraRelativeQuaternion = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up);
            var direction = cameraRelativeQuaternion * _moveDirection;
            var directionMagnitude = direction.magnitude;
            if (directionMagnitude > 1)
            {
                direction.Normalize();
                directionMagnitude = SpeedModifier;
            }
            else
            {
                directionMagnitude = Mathf.Clamp(directionMagnitude, 0, SpeedModifier);
            }
            if (!_useRootMotion)
            {
                var maxSpeed = _movementCtx.IsGrounded ? _maxSpeed : _maxInAirSpeed;
                float speed = directionMagnitude * maxSpeed;
                var inputVelocity = speed * direction;
                inputVelocity = new(inputVelocity.x, _fallSpeed, inputVelocity.z);
                _movementCtx.Velocity = inputVelocity;
            }
            else
            {
                _movementCtx.Velocity = new(0, _fallSpeed, 0);
            }
            _movementCtx.Position = _characterController.transform.position;
            var wasGrounded = _movementCtx.IsGrounded;

            _slopeSlideOptions.MinSlidingSlopeAngle = _characterController.slopeLimit;
            _slopeSlideOptions.MaxDistanceToSlope = _maxDistanceToSlope;
            _slopeSlideOptions.Friction = _slopeSlideFriction;
            _slopeSlideOptions.BrakingFriction = _slopeSlideBrakingFriction;
            _slopeSlideMovement.Update(_movementCtx);
            _jumpMovement.Update(_movementCtx);

            float gravityAcceleration = Mathf.Abs(Physics.gravity.y) * _gravityMultiplier * _gravityModifier;

            _fallSpeed = _movementCtx.Velocity.y;
            if (wasGrounded != _movementCtx.IsGrounded)
            {
                if (_movementCtx.IsGrounded)
                {
                    _lastGroundedTime = Time.time;
                    _movementCtx.IsGroundedCoyote = true;
                }
                else
                {
                    _lastGroundedTime = null;
                    _movementCtx.IsGroundedCoyote = false;
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
            if (TryUpdateRotation(cameraRelativeQuaternion, direction, _lookDirection, out var rotation))
            {
                _characterController.transform.rotation = rotation.Value;
            }

            if (_movementCtx.IsGrounded && _movementCtx.IsGroundStable)
            {
                _fallSpeed = -2f; // to prevent isGrounded false positives
            }
            else if (!_movementCtx.IsGrounded)
            {
                _fallSpeed -= gravityAcceleration * Time.deltaTime;
            }

            UpdateAnimationsIfExists(direction, directionMagnitude);
        }

        protected virtual void OnAnimatorMove()
        {
            if (_useRootMotion && _animator != null && _movementCtx.IsGrounded && _movementCtx.IsGroundStable)
            {
                Vector3 velocity = _animator.deltaPosition;
                velocity.y = _fallSpeed * Time.deltaTime;
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
        [SerializeField, Range(0, 1)]
        private float _speedModifier = 1f;
        [SerializeField, Range(0, 1)]
        private float _gravityModifier = 1f;
        [SerializeField]
        private float _gravityMultiplier = 1.5f;
        [SerializeField, Range(0, 1)]
        private float _slopeSlideFriction = 0.3f;
        [SerializeField, Range(0, 1)]
        private float _slopeSlideBrakingFriction = 1f;
        [SerializeField]
        private float _maxDistanceToSlope = 2f;
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

        private readonly SlopeSlideMovementOptions _slopeSlideOptions = new(0, 0, 0, Physics.gravity.y, 0);
        private readonly MovementContext _movementCtx = new(Vector3.zero, Vector3.zero, true, true, true);
        private SlopeSlideMovement _slopeSlideMovement;
        private JumpMovement _jumpMovement;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _lookDirection = Vector2.zero;
        private float _fallSpeed;
        private float _characterControllerStepOffset;
        private float? _lastGroundedTime;

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

        private void UpdateAnimationsIfExists(Vector3 direction, float directionMagnitude)
        {
            if (_animator != null)
            {
                var isStableGrounded = _movementCtx.IsGrounded && _movementCtx.IsGroundStable;
                _animator.SetFloat(InputMagnitudeHash.Value, directionMagnitude, 0.5f, Time.deltaTime);
                _animator.SetBool(IsGroundedHash.Value, isStableGrounded);
                _animator.SetBool(IsJumpingHash.Value, _jumpMovement.IsActive);
                _animator.SetBool(IsFallingHash.Value, !isStableGrounded);
                _animator.SetBool(IsMovingHash.Value, direction != Vector3.zero);
                _animator.SetBool(IsLookingHash.Value, _lookDirection != Vector3.zero);
            }
        }
    }
}

using KarenKrill.UniCore.Utilities;
using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public class CharacterMoveBehaviour : MonoBehaviour
    {
        public float MaximumSpeed { get => _maximumSpeed; set => _maximumSpeed = value; }
        /// <summary>Range: [0..1]</summary>
        public float SpeedModifier { get => _speedModifier; set => _speedModifier = value; }
        /// <summary>Range: [0..1]</summary>
        public float GravityModifier { get => _gravityModifier; set => _gravityModifier = value; }
        public bool IsGrounded => _characterController.isGrounded;
        public bool IsSliding => _slopeSlideMovement.IsActive;
        public bool IsFalling => _fallSpeed > 0;
        public bool IsPulsedUp => _isPulsedUp;

        public bool EnableCharController { get => _characterController.enabled; set => _characterController.enabled = value; }
        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
        public Vector2 LookDirection { get => _lookDirection; set => _lookDirection = value; }

        public void PulseUp(float distance, float gracePeriod, float inAirHorizontalSpeed)
        {
            _pulseUpDistance = distance;
            _pulseUpStartTime = Time.time;
            _pulseUpGracePeriod = gracePeriod;
            _inAirHorizontalSpeed = inAirHorizontalSpeed;
        }

        protected virtual void Awake()
        {
            _slopeSlideMovement = new(_slopeSlideOptions);
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
            UpdateMovement();
        }
        protected virtual void OnAnimatorMove()
        {
            if (_useRootMotion && _animator != null && _isGrounded && !_slopeSlideMovement.IsActive)
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
        private float _maximumSpeed = 5f, _rotationDegreeSpeed = 360.0f;
        [SerializeField, Range(0, 1)]
        private float _speedModifier = 1f;
        [SerializeField, Range(0, 1)]
        private float _gravityModifier = 1f;
        [SerializeField]
        private float _gravityMultiplier = 1.5f;
        [SerializeField]
        private float _slidingDecelerationFactor = 3f;
        [SerializeField]
        private bool _useRootMotion = false;
        /// <summary>
        /// Use MoveDirection towards or LookDirection for character rotation
        /// </summary>
        [SerializeField]
        private bool _thirdPerson = false;

        private readonly SlopeSlideMovementOptions _slopeSlideOptions = new(0, 0);
        private readonly SlopeSlideMovementContext _slopeSlideCtx = new(Vector3.zero, 0);
        private SlopeSlideMovement _slopeSlideMovement;

        private bool _isPulsedUp = false, _isGrounded = false;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _lookDirection = Vector2.zero;
        private float _fallSpeed;
        private float _pulseUpGracePeriod = 0.2f;
        private float _pulseUpDistance = 2.0f, _inAirHorizontalSpeed = 3.0f;
        private float _characterControllerStepOffset;
        private float? _lastGroundedTime, _pulseUpStartTime;

        private void UpdateMovement()
        {
            float gravity = Physics.gravity.y * _gravityMultiplier * _gravityModifier;
            _fallSpeed += gravity * Time.deltaTime;

            _slopeSlideOptions.SlopeLimitDegrees = _characterController.slopeLimit;
            _slopeSlideOptions.DecelerationFactor = _slidingDecelerationFactor;
            _slopeSlideCtx.TargetPosition = _characterController.transform.position;
            _slopeSlideCtx.TargetFallSpeed = _fallSpeed;
            _slopeSlideMovement.Update(_slopeSlideCtx);

            _isGrounded = _characterController.isGrounded;
            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;
            }

            // Check on falling
            if (Time.time - _lastGroundedTime <= _pulseUpGracePeriod) // grounded recently
            {
                _characterController.stepOffset = _characterControllerStepOffset;
                _isGrounded = true;
                _isPulsedUp = false;
                if (!_slopeSlideMovement.IsActive)
                {
                    if (Time.time - _pulseUpStartTime <= _pulseUpGracePeriod) // pulsed up recently
                    {
                        _isPulsedUp = true;
                        _pulseUpStartTime = null;
                        _lastGroundedTime = null;
                        _fallSpeed = Mathf.Sqrt(_pulseUpDistance * -3 * gravity);
                    }
                    else
                    {
                        _fallSpeed = -0.5f; // to prevent isGrounded false positives
                    }
                }
            }
            else
            {
                _characterController.stepOffset = 0; // fix stuck in the wall while jumping
                if ((_isPulsedUp && _fallSpeed < 0) || _fallSpeed < -2)
                {
                    _isGrounded = false;
                }
            }

            // Direction & DirectionInputMagnitude usings
            var cameraRelativeQuaternion = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up);
            var direction = cameraRelativeQuaternion * _moveDirection;
            if (direction.magnitude > 1)
            {
                direction.Normalize();
            }
            var directionMagnitude = Mathf.Clamp(direction.magnitude, 0, SpeedModifier);
            if (!_useRootMotion)
            {
                float speed = directionMagnitude * _maximumSpeed;
                Vector3 velocity = speed * direction;
                velocity.y = _fallSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }
            if (_slopeSlideMovement.IsActive)
            {
                Vector3 velocity = _slopeSlideMovement.Velocity;
                velocity.y = _fallSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }
            else if (!_isGrounded) // jumping
            {
                float speed = directionMagnitude * _inAirHorizontalSpeed;
                Vector3 velocity = speed * direction;
                velocity.y = _fallSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }

            // Direction usings
            bool isMoving = direction != Vector3.zero;
            bool isLooking = _lookDirection != Vector3.zero;
            if (_thirdPerson)
            {
                if (isMoving)
                {
                    var directionLookRotation = Quaternion.LookRotation(direction, Vector3.up);
                    var characterRotation = _characterController.transform.rotation;
                    var rotation = Quaternion.RotateTowards(characterRotation, directionLookRotation, _rotationDegreeSpeed * Time.deltaTime);
                    _characterController.transform.rotation = rotation;
                }
            }
            else if (isMoving || isLooking)
                {
                    var rotation = Quaternion.RotateTowards(_characterController.transform.rotation, cameraRelativeQuaternion, 360);
                    _characterController.transform.rotation = rotation;
                }

            if (_animator != null)
            {
                _animator.SetFloat(InputMagnitudeHash.Value, directionMagnitude, 0.5f, Time.deltaTime);
                _animator.SetBool(IsGroundedHash.Value, _isGrounded);
                _animator.SetBool(IsJumpingHash.Value, _isPulsedUp);
                _animator.SetBool(IsFallingHash.Value, !_isGrounded);
                _animator.SetBool(IsMovingHash.Value, isMoving);
                _animator.SetBool(IsLookingHash.Value, isLooking);
            }
        }

        
    }
}

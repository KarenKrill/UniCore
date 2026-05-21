#nullable enable
using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class SlopeSlideMovement : IMoveAbility
    {
        public static readonly float MinSlidingSlopeAngleDefault = 45;
        public static readonly float FrictionDefault = 0.3f;
        public static readonly float BrakingFrictionDefault = 1;
        public static readonly float MaxDistanceToSlopeDefault = 10;

        public bool IsActive => _state.IsSliding;

        [field: SerializeField]
        public bool Enabled { get; set; } = true;

        public SlopeSlideMovementOptions Options => _options;

        public SlopeSlideMovement()
        {
            _options = new(MinSlidingSlopeAngleDefault, FrictionDefault, BrakingFrictionDefault, MaxDistanceToSlopeDefault);
        }
        public SlopeSlideMovement(SlopeSlideMovementOptions options)
        {
            _options = options;
        }

        public void Update(MovementContext ctx)
        {
            if (Enabled && (ctx.IsGroundedCoyote || _state.IsSliding))
            {
                if (IsPlacedOnSurface(ctx.Position))
                {
                    var rayHit = _downRaycastHits[0];
                    var angleToSurface = GetAngleToSurface(rayHit.normal);
                    var isSlidingAngle = angleToSurface > _options.MinSlidingSlopeAngle;
                    if (isSlidingAngle)
                    {
                        if (!_state.IsSliding)
                        {
                            _state.IsSliding = true;
                            _state.Velocity = Vector3.zero;
                            _state.SlideDownSpeed = -Mathf.Min(ctx.Velocity.y, 0); // take Y if it negative
                            ctx.IsGroundStable = false;
                        }
                    }
                    else if (_state.IsSliding)
                    {
                        if (_state.Velocity.magnitude < 1)
                        {
                            _state.IsSliding = false;
                            ctx.IsGroundStable = true;
                        }
                    }
                    if(_state.IsSliding)
                    {
                        var acceleration = GetSlideAcceleration(angleToSurface, isSlidingAngle, ctx.Gravity);
                        var deltaSpeed = acceleration * Time.deltaTime;
                        var slideDirection = GetSlideVelocityDirection(rayHit.normal);
                        if (slideDirection != Vector3.zero)
                        {
                            _state.LastValidSlideDirection = slideDirection;
                        }
                        _state.Velocity = _state.LastValidSlideDirection * _state.SlideDownSpeed;
                        _state.SlideDownSpeed += deltaSpeed;
                        ctx.Velocity = _state.Velocity;
                    }
                }
                else if (_state.IsSliding)
                {
                    _state.IsSliding = false;
                    ctx.IsGroundStable = true;
                }
            }
        }

        [SerializeField]
        private SlopeSlideMovementOptions _options;

        private readonly SlopeSlideMovementState _state = new();
        private readonly RaycastHit[] _downRaycastHits = new RaycastHit[1];

        private static float GetAngleToSurface(Vector3 normal) => Vector3.Angle(normal, Vector3.up);

        private static Vector3 GetSlideVelocityDirection(Vector3 normal)
        {
            return Vector3.ProjectOnPlane(Vector3.down, normal).normalized;
        }

        private bool IsPlacedOnSurface(Vector3 position)
        {
            return Physics.RaycastNonAlloc(position, Vector3.down, _downRaycastHits, _options.MaxDistanceToSlope) > 0;
        }

        private float GetSlideAcceleration(float slopeAngle, bool isSlidingAngle, float gravity)
        {
            var slopeAngleRad = slopeAngle * Mathf.Deg2Rad;
            var friction = isSlidingAngle ? _options.Friction : _options.BrakingFriction;
            return -gravity * (Mathf.Sin(slopeAngleRad) - friction * Mathf.Cos(slopeAngleRad));
        }
    }
}

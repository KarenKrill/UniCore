#nullable enable
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public class SlopeSlideMovement
    {
        public static readonly float MinSlidingSlopeAngleDefault = 45;
        public static readonly float DecelerationFactorDefault = 1;

        public bool IsActive => _state.IsSliding;
        public Vector3 Velocity => _state.Velocity;

        public SlopeSlideMovement(SlopeSlideMovementOptions? options = null)
        {
            _options = options ?? new(MinSlidingSlopeAngleDefault, DecelerationFactorDefault, Physics.gravity.y, 0);
        }

        public void Update(SlopeSlideMovementContext ctx)
        {
            if (ctx.IsGroundedCoyote || _state.IsSliding)
            {
                if (IsPlacedOnSurface(ctx.Position))
                {
                    var rayHit = _downRaycastHits[0];
                    var angleToSurface = GetAngleToSurface(rayHit.normal);
                    if (angleToSurface > _options.MinSlidingSlopeAngle)
                    {
                        if (!_state.IsSliding)
                        {
                            _state.IsSliding = true;
                            ctx.IsGroundStable = false;
                            // get current Y velocity if it negative
                            _slideDownSpeed = Mathf.Max(-ctx.Velocity.y, 0);
                        }
                        var velocityDirection = GetSlideVelocityDirection(rayHit.normal);
                        _state.Velocity = velocityDirection * _slideDownSpeed;
                        _slideDownSpeed += GetSlideAcceleration(angleToSurface) * Time.deltaTime;
                        return;
                    }
                }
                if (_state.IsSliding)
                {
                    ctx.IsGroundStable = true;
                    _state.Velocity -= _options.DecelerationFactor * Time.deltaTime * _state.Velocity;
                    if (_state.Velocity.magnitude > 1)
                    {
                        return;
                    }
                    _state.Velocity = Vector3.zero;
                    _state.IsSliding = false;
                }
            }
        }

        private readonly SlopeSlideMovementState _state = new();
        private readonly SlopeSlideMovementOptions _options;
        private readonly RaycastHit[] _downRaycastHits = new RaycastHit[1];
        private float _slideDownSpeed;
        private float _frictionCoefficient;
        private float _lastSlopeLimitDegrees;

        private static float GetAngleToSurface(Vector3 normal) => Vector3.Angle(normal, Vector3.up);

        private static Vector3 GetSlideVelocityDirection(Vector3 normal)
        {
            return Vector3.ProjectOnPlane(Vector3.down, normal).normalized;
        }

        private bool IsPlacedOnSurface(Vector3 position)
        {
            return Physics.RaycastNonAlloc(position, Vector3.down, _downRaycastHits, _options.MaxDistanceToSlope) > 0;
        }

        private float GetFriction()
        {
            if (_options.MinSlidingSlopeAngle != _lastSlopeLimitDegrees)
            {
                _lastSlopeLimitDegrees = _options.MinSlidingSlopeAngle;
                _frictionCoefficient = Mathf.Tan(_lastSlopeLimitDegrees * Mathf.Deg2Rad);
            }
            return _frictionCoefficient;
        }

        private float GetSlideAcceleration(float slopeAngle)
        {
            var slopeAngleRad = slopeAngle * Mathf.Deg2Rad; 
            var frictionCoefficient = GetFriction();
            return -_options.Gravity * (Mathf.Sin(slopeAngleRad) - frictionCoefficient * Mathf.Cos(slopeAngleRad));
        }
    }
}

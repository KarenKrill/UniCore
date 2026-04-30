#nullable enable
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public class SlopeSlideMovement
    {
        public static readonly float SlopeLimitDegreesDefault = 45;
        public static readonly float DecelerationFactorDefault = 1;

        public bool IsActive => _state.IsSliding;
        public Vector3 Velocity => _state.Velocity;

        public SlopeSlideMovement(SlopeSlideMovementOptions? options = null)
        {
            _options = options ?? new(SlopeLimitDegreesDefault, DecelerationFactorDefault);
        }

        public void Update(SlopeSlideMovementContext ctx)
        {
            if (Physics.Raycast(ctx.TargetPosition, Vector3.down, out var hitInfo))
            {
                float slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
                if (slopeAngle >= _options.SlopeLimitDegrees)
                {
                    _state.Velocity = Vector3.ProjectOnPlane(new Vector3(0, ctx.TargetFallSpeed, 0), hitInfo.normal);
                    _state.IsSliding = true;
                    return;
                }
            }
            if (_state.IsSliding)
            {
                _state.Velocity -= _options.DecelerationFactor * Time.deltaTime * _state.Velocity;
                if (_state.Velocity.magnitude > 1)
                {
                    return;
                }
            }
            _state.Velocity = Vector3.zero;
            _state.IsSliding = false;
        }

        private readonly SlopeSlideMovementState _state = new();
        private readonly SlopeSlideMovementOptions _options;
    }
}

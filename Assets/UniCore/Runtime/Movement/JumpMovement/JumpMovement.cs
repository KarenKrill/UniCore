#nullable enable
using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class JumpMovement : IMoveAbility
    {
        public bool IsActive => _isPulsedUp;

        [field: SerializeField]
        public bool Enabled { get; set; } = true;

        public void PulseUp(float distance, float gracePeriod, float gravity)
        {
            _pulseUpDistance = distance;
            _pulseUpStartTime = Time.time;
            _pulseUpGracePeriod = gracePeriod;
            _gravity = gravity;
        }
        public void Update(MovementContext ctx)
        {
            if (Enabled && ctx.IsGroundedCoyote)
            {
                if (ctx.IsGroundStable)
                {
                    //ctx.IsGrounded = true;
                    _isPulsedUp = false;
                    if (Time.time - _pulseUpStartTime <= _pulseUpGracePeriod) // pulsed up recently
                    {
                        ctx.IsGrounded = false;
                        _isPulsedUp = true;
                        _pulseUpStartTime = null;
                        var speed = Mathf.Sqrt(2 * _pulseUpDistance * _gravity);
                        var velocity = new Vector3(ctx.Velocity.x, speed, ctx.Velocity.z);
                        ctx.Velocity = velocity;
                    }
                }
            }
            /*else if ((_isPulsedUp && ctx.Velocity.y < 0) || ctx.Velocity.y < -2f)
            {
                ctx.IsGrounded = false;
                //_isPulsedUp = false;
            }*/
        }

        private bool _isPulsedUp = false;

        private float _pulseUpGracePeriod = 0.2f;
        private float _pulseUpDistance = 2.0f;
        private float? _pulseUpStartTime;
        private float _gravity;
    }
}
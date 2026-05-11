#nullable enable
using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class JumpMovement : IMoveAbility
    {
        public static readonly float JumpHeightDefault = 2;
        public static readonly float GracePeriodDefault = 0.2f;
        public static readonly float LongJumpHoldTimeDefault = 0.2f;

        public bool IsActive => _isJumping;

        [field: SerializeField]
        public bool Enabled { get; set; } = true;

        public JumpMovementOptions Options => _options;

        public JumpMovement() { _options = new(JumpHeightDefault, GracePeriodDefault, LongJumpHoldTimeDefault); }

        public JumpMovement(JumpMovementOptions options) { _options = options; }

        public void Jump()
        {
            _jumpStartTime = Time.time;
            _isPowerfulJump = true;
        }

        public void JumpCancel()
        {
            if (Time.time - _jumpStartTime <= _options.PowerfulJumpHoldTime)
            {
                _isPowerfulJump = false;
            }
        }

        public void Update(MovementContext ctx)
        {
            if (_isJumping)
            {
                if (!ctx.IsGrounded && _isPowerfulJump)
                {
                    ctx.GravityModifier = _options.WeakJumpIntensity; // reduces gravity resistance to jumping (increases jump power)
                }
                else
                {
                    ctx.GravityModifier = 1f;
                    _isJumping = false;
                }
            }
            else if (ctx.IsGroundStable && ctx.IsGroundedCoyote)
            {
                if (Time.time - _jumpStartTime <= _options.GracePeriod) // jump requested recently
                {
                    _isJumping = true;
                    ctx.IsGrounded = false;
                    // Reset last ground time to prevent multiple jumps within Coyote time
                    ctx.LastGroundedTime = float.MinValue;
                    var verticalSpeed = Mathf.Sqrt(2 * _options.MaxHeight * Mathf.Abs(ctx.Gravity) * _options.WeakJumpIntensity);
                    if(ctx.Gravity > 0)
                    {
                        verticalSpeed *= -1;
                    }
                    ctx.Velocity = new(ctx.Velocity.x, verticalSpeed, ctx.Velocity.z);
                }
            }
        }

        [SerializeField]
        private JumpMovementOptions _options;

        private bool _isJumping = false;
        private float _jumpStartTime = float.MinValue;
        private bool _isPowerfulJump = true;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public class MovementContext
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

        /// <summary>Gravity acceleration</summary>
        /// <remarks>In normal cases, it should be negative</remarks>
        public float Gravity { get; set; }
        [Min(0)]
        public float GravityModifier { get; set; }

        public bool IsGrounded
        {
            get => _isGrounded;
            set
            {
                _isGrounded = value;
                if (_isGrounded)
                {
                    _lastGroundedTime = Time.time;
                }
            }
        }

        public float CoyoteTime { get; set; }

        public float LastGroundedTime { get => _lastGroundedTime; set => _lastGroundedTime = value; }

        /// <summary>Latent version of <see cref="IsGrounded"/></summary>
        /// <remarks>
        /// Technique, that grants players a tiny "grace period" to do something after walking off a platform
        /// </remarks>
        public bool IsGroundedCoyote => Time.time - _lastGroundedTime <= CoyoteTime;

        public bool IsGroundStable { get; set; }

        public MovementContext(Vector3 position, Vector3 velocity,
            float gravity, float gravityModifier = 1,
            bool isGrounded = false, bool isGroundStable = false,
            float coyoteTime = .5f, float lastGroundedTime = float.MinValue)
        {
            Position = position;
            Velocity = velocity;
            Gravity = gravity;
            GravityModifier = gravityModifier;
            _isGrounded = isGrounded;
            _lastGroundedTime = lastGroundedTime;
            IsGroundStable = isGroundStable;
            CoyoteTime = coyoteTime;
        }

        public T GetUserContext<T>()
        {
            return (T)_userContexts[typeof(T)];
        }

        public void SetUserContext<T>(T context)
        {
            _userContexts[typeof(T)] = context;
        }

        public void SetUserContext(object context)
        {
            _userContexts[context.GetType()] = context;
        }

        private readonly Dictionary<Type, object> _userContexts = new();
        private bool _isGrounded = false;
        private float _lastGroundedTime = float.MinValue;
    }
}

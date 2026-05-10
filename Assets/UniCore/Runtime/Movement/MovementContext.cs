using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public class MovementContext
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

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

        public MovementContext(Vector3 position, Vector3 velocity, bool isGrounded = false, bool isGroundStable = false, float coyoteTime = .5f, float lastGroundedTime = float.MinValue)
        {
            Position = position;
            Velocity = velocity;
            _isGrounded = isGrounded;
            _lastGroundedTime = lastGroundedTime;
            IsGroundStable = isGroundStable;
            CoyoteTime = coyoteTime;
        }

        private bool _isGrounded = false;
        private float _lastGroundedTime = float.MinValue;
    }
}

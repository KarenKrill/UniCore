using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class MovementContext
    {
        [field: SerializeField]
        public Vector3 Position { get; set; }

        [field: SerializeField]
        public Vector3 Velocity { get; set; }

        [field: SerializeField]
        public bool IsGrounded { get; set; }

        /// <summary>Latent version of <see cref="IsGrounded"/></summary>
        /// <remarks>
        /// Technique, that grants players a tiny "grace period" to do something after walking off a platform
        /// </remarks>
        [field: SerializeField]
        public bool IsGroundedCoyote { get; set; }

        [field: SerializeField]
        public bool IsGroundStable { get; set; }

        public MovementContext(Vector3 position, Vector3 velocity, bool isGrounded = false, bool isGroundedCoyote = false, bool isGroundStable = false)
        {
            Position = position;
            Velocity = velocity;
            IsGrounded = isGrounded;
            IsGroundedCoyote = isGroundedCoyote;
            IsGroundStable = isGroundStable;
        }
    }
}

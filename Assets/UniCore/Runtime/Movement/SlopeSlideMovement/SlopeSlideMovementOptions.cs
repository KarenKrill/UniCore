using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class SlopeSlideMovementOptions
    {
        /// <summary>
        /// Minimum slope angle (in degrees) at which sliding begins
        /// </summary>
        [field: SerializeField]
        public float MinSlidingSlopeAngle { get; set; }

        /// <summary>
        /// The coefficient of friction of a character sliding down a slope. Affects the character's sliding speed.
        /// </summary>
        [field: SerializeField, Range(0, 1)]
        public float Friction { get; set; }

        /// <summary>
        /// The coefficient of friction when the character brakes after sliding down a slope. Affects the character's braking speed.
        /// </summary>
        [field: SerializeField, Range(0, 1)]
        public float BrakingFriction { get; set; }

        /// <summary>Gravity acceleration</summary>
        /// <remarks>In normal cases, it should be negative</remarks>
        [field: SerializeField]
        public float Gravity { get; set; }

        /// <summary>
        /// The maximum distance under a character at which the search for an slope surface occurs
        /// </summary>
        [field: SerializeField]
        public float MaxDistanceToSlope { get; set; }

        public SlopeSlideMovementOptions(float slopeLimitDegrees, float friction, float brakingFriction, float gravity, float maxDistanceToSlope)
        {
            MinSlidingSlopeAngle = slopeLimitDegrees;
            Friction = friction;
            BrakingFriction = brakingFriction;
            Gravity = gravity;
            MaxDistanceToSlope = maxDistanceToSlope;
        }
    }
}

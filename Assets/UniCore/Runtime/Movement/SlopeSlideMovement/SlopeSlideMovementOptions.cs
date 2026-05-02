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
        /// A coefficient that affects the speed at which a character's inertial movement stops after sliding
        /// </summary>
        [field: SerializeField]
        public float DecelerationFactor { get; set; }

        /// <summary>Gravity acceleration</summary>
        /// <remarks>In normal cases, it should be negative</remarks>
        [field: SerializeField]
        public float Gravity { get; set; }

        /// <summary>
        /// The maximum distance under a character at which the search for an slope surface occurs
        /// </summary>
        [field: SerializeField]
        public float MaxDistanceToSlope { get; set; }

        public SlopeSlideMovementOptions(float slopeLimitDegrees, float decelerationFactor, float gravity, float maxDistanceToSlope)
        {
            MinSlidingSlopeAngle = slopeLimitDegrees;
            DecelerationFactor = decelerationFactor;
            Gravity = gravity;
            MaxDistanceToSlope = maxDistanceToSlope;
        }
    }
}

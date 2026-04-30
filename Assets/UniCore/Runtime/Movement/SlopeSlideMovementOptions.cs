using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class SlopeSlideMovementOptions
    {
        [field: SerializeField]
        public float SlopeLimitDegrees { get; set; }
        [field: SerializeField]
        public float DecelerationFactor { get; set; }

        public SlopeSlideMovementOptions(float slopeLimitDegrees, float decelerationFactor)
        {
            SlopeLimitDegrees = slopeLimitDegrees;
            DecelerationFactor = decelerationFactor;
        }
    }
}

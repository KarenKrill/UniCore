using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class SlopeSlideMovementContext
    {
        [field: SerializeField]
        public Vector3 TargetPosition { get; set; }
        [field: SerializeField]
        public float TargetFallSpeed { get; set; }

        public SlopeSlideMovementContext(Vector3 targetPosition, float targetFallSpeed)
        {
            TargetPosition = targetPosition;
            TargetFallSpeed = targetFallSpeed;
        }
    }
}

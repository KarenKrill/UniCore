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
        [field: SerializeField]
        public bool IsGroundStable { get; set; }

        public SlopeSlideMovementContext(Vector3 targetPosition, float targetFallSpeed, bool isGroundStable)
        {
            TargetPosition = targetPosition;
            TargetFallSpeed = targetFallSpeed;
            IsGroundStable = isGroundStable;
        }
    }
}

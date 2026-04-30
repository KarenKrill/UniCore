using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class SlopeSlideMovementState
    {
        [field: SerializeField]
        public bool IsSliding { get; set; } = false;
        [field: SerializeField]
        public Vector3 Velocity { get; set; } = Vector3.zero;
    }
}

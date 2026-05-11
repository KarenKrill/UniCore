using System;
using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    [Serializable]
    public class JumpMovementOptions
    {
        [field: SerializeField]
        public float MaxHeight { get; set; } = 2f;

        /// <summary>
        /// The hold time for a jump request in case activation is not available at the moment
        /// </summary>
        /// <remarks>
        /// For example, if a jump request comes when the character is in the air, then a delayed jump will be performed if the character manages to land on the ground during the hold time
        /// </remarks>
        [field: SerializeField]
        public float GracePeriod { get; set; } = 0.2f;

        [field: SerializeField]
        public float PowerfulJumpHoldTime { get; set; } = 0.2f;

        [field: SerializeField, Range(0, 1)]
        public float WeakJumpIntensity { get; set; } = 0.5f;

        public JumpMovementOptions(float maxHeight, float gracePeriod = 0.2f, float powerfulJumpHoldTime = 0.2f, float weakJumpIntensity = 0.5f)
        {
            MaxHeight = maxHeight;
            GracePeriod = gracePeriod;
            PowerfulJumpHoldTime = powerfulJumpHoldTime;
            WeakJumpIntensity = weakJumpIntensity;
        }
    }
}

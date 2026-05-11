using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public class MoveInputContext
    {
        public Vector2 LookDelta { get; set; } = Vector2.zero;
        public Vector2 MoveDelta { get; set; } = Vector2.zero;

        public bool IsSprintPressed { get; set; } = false;
        public bool IsCrouchPressed { get; set; } = false;
        public bool IsJumpPressed { get; set; } = false;

        public float LastSprintStartTime { get; set; } = float.MinValue;
        public float LastCrouchStartTime { get; set; } = float.MinValue;
        public float LastJumpStartTime { get; set; } = float.MinValue;
    }
}

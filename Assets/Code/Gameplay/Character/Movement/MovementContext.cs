using UnityEngine;

namespace Game.Gameplay.Character.Movement
{
    /// <summary>
    /// Movement-specific context data.
    /// Part of CharacterContext.
    /// </summary>
    public class MovementContext
    {
        // Input from driver
        public Vector3 MoveInput { get; set; }
        public Vector2 LookInput { get; set; }

        // Pending impulse to apply in next UpdateVelocity
        public Vector3 PendingImpulse { get; set; }

        // Speed multiplier (set by abilities like Sprint)
        public float SpeedMultiplier { get; set; } = 1f;

        public void AddImpulse(Vector3 impulse)
        {
            PendingImpulse += impulse;
        }

        public Vector3 ConsumeImpulse()
        {
            var impulse = PendingImpulse;
            PendingImpulse = Vector3.zero;
            return impulse;
        }

        public void ResetFrame()
        {
            SpeedMultiplier = 1f;
        }
    }
}

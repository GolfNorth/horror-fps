using KinematicCharacterController;
using UnityEngine;

namespace Game.Gameplay.Character.Abilities
{
    public class GravityAbility : MovementAbility
    {
        public override int Priority => 0;

        [SerializeField] private float _gravityMultiplier = 2f;
        [SerializeField] private float _fallMultiplier = 2.5f;
        [SerializeField] private float _maxFallSpeed = 20f;

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (motor.GroundingStatus.IsStableOnGround)
                return false;

            var multiplier = currentVelocity.y < 0f ? _fallMultiplier : _gravityMultiplier;
            currentVelocity += Physics.gravity * (multiplier * deltaTime);

            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (verticalSpeed < -_maxFallSpeed)
            {
                currentVelocity += motor.CharacterUp * (-_maxFallSpeed - verticalSpeed);
            }

            return false;
        }
    }
}

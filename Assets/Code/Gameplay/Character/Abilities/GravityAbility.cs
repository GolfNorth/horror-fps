using Game.Gameplay.Character.Configs;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class GravityAbility : MovementAbility
    {
        public override int Priority => 0;

        private CharacterMovementConfig _config;

        [Inject]
        public void Construct(CharacterMovementConfig config)
        {
            _config = config;
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_config == null) return false;
            if (motor.GroundingStatus.IsStableOnGround)
                return false;

            var multiplier = currentVelocity.y < 0f ? _config.FallMultiplier : _config.GravityMultiplier;
            currentVelocity += Physics.gravity * (multiplier * deltaTime);

            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (verticalSpeed < -_config.MaxFallSpeed)
            {
                currentVelocity += motor.CharacterUp * (-_config.MaxFallSpeed - verticalSpeed);
            }

            return false;
        }
    }
}

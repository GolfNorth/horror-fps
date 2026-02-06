using Game.Core.Configuration;
using KinematicCharacterController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class GravityAbility : MovementAbility, IInitializable
    {
        public override int Priority => 0;

        private IConfigService _config;
        private IConfigValue<float> _gravityMultiplier;
        private IConfigValue<float> _fallMultiplier;
        private IConfigValue<float> _maxFallSpeed;

        [Inject]
        public void Construct(IConfigService config)
        {
            _config = config;
        }

        public void Initialize()
        {
            _gravityMultiplier = _config.Observe<float>("gravity.multiplier");
            _fallMultiplier = _config.Observe<float>("gravity.fall_multiplier");
            _maxFallSpeed = _config.Observe<float>("gravity.max_fall_speed");
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_gravityMultiplier == null) return false;
            if (motor.GroundingStatus.IsStableOnGround)
                return false;

            var multiplier = currentVelocity.y < 0f ? _fallMultiplier.Value : _gravityMultiplier.Value;
            currentVelocity += Physics.gravity * (multiplier * deltaTime);

            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (verticalSpeed < -_maxFallSpeed.Value)
            {
                currentVelocity += motor.CharacterUp * (-_maxFallSpeed.Value - verticalSpeed);
            }

            return false;
        }
    }
}

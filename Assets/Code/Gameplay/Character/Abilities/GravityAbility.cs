using Game.Core.Configuration;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class GravityAbility : MovementAbility
    {
        public override int Priority => 0;

        [SerializeField] private string _characterId = "player";

        private IConfigValue<float> _gravityMultiplier;
        private IConfigValue<float> _fallMultiplier;
        private IConfigValue<float> _maxFallSpeed;

        [Inject]
        public void Construct(IConfigService config)
        {
            var id = _characterId;
            _gravityMultiplier = config.Observe<float>($"{id}.gravity.multiplier");
            _fallMultiplier = config.Observe<float>($"{id}.gravity.fall_multiplier");
            _maxFallSpeed = config.Observe<float>($"{id}.gravity.max_fall_speed");
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

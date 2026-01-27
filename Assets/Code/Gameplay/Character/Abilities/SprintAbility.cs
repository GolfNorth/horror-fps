using Game.Gameplay.Character.Configs;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class SprintAbility : MovementAbility
    {
        public override int Priority => 5;

        private CharacterMovementConfig _config;
        private bool _wantsToSprint;

        public bool IsSprinting => _wantsToSprint;

        [Inject]
        public void Construct(CharacterMovementConfig config)
        {
            _config = config;
        }

        public void SetSprintInput(bool sprint)
        {
            _wantsToSprint = sprint;
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_config == null) return false;
            if (!_wantsToSprint || !motor.GroundingStatus.IsStableOnGround)
                return false;

            var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
            var currentSpeed = horizontal.magnitude;

            if (currentSpeed < 0.01f)
                return false;

            var targetSpeed = Mathf.MoveTowards(currentSpeed, _config.SprintSpeed, _config.SprintAcceleration * deltaTime);
            var vertical = Vector3.Project(currentVelocity, motor.CharacterUp);

            currentVelocity = horizontal.normalized * targetSpeed + vertical;

            return false;
        }
    }
}

using Game.Core.Configuration;
using Game.Gameplay.Character.Actions;
using KinematicCharacterController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class SprintAbility : MovementAbility, IInitializable
    {
        public override int Priority => 5;

        private IActionBuffer _actions;
        private CharacterState _state;
        private IConfigService _config;
        private IConfigValue<float> _sprintSpeed;
        private IConfigValue<float> _sprintAcceleration;

        [Inject]
        public void Construct(IConfigService config, IActionBuffer actions, CharacterState state)
        {
            _config = config;
            _actions = actions;
            _state = state;
        }

        public void Initialize()
        {
            _sprintSpeed = _config.Observe<float>("sprint.speed");
            _sprintAcceleration = _config.Observe<float>("sprint.acceleration");
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_sprintSpeed == null) return false;

            var wantsToSprint = _actions.Has<SprintAction>();
            _state.IsSprinting.Value = wantsToSprint && motor.GroundingStatus.IsStableOnGround;

            if (!wantsToSprint || !motor.GroundingStatus.IsStableOnGround)
                return false;

            var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
            var currentSpeed = horizontal.magnitude;

            if (currentSpeed < 0.01f)
                return false;

            var targetSpeed = Mathf.MoveTowards(currentSpeed, _sprintSpeed.Value, _sprintAcceleration.Value * deltaTime);
            var vertical = Vector3.Project(currentVelocity, motor.CharacterUp);

            currentVelocity = horizontal.normalized * targetSpeed + vertical;

            return false;
        }
    }
}

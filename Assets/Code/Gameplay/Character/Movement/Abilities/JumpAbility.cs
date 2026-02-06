using Game.Core.Configuration;
using Game.Gameplay.Character.Actions;
using KinematicCharacterController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class JumpAbility : MovementAbility, IInitializable
    {
        public override int Priority => 30;

        private IActionBuffer _actions;
        private IConfigService _config;
        private IConfigValue<float> _jumpForce;
        private IConfigValue<float> _coyoteTime;
        private IConfigValue<float> _bufferTime;

        private bool _jumpConsumed;
        private float _timeSinceJumpRequested = float.MaxValue;
        private float _timeSinceLastGrounded = float.MaxValue;

        [Inject]
        public void Construct(IConfigService config, IActionBuffer actions)
        {
            _config = config;
            _actions = actions;
        }

        public void Initialize()
        {
            _jumpForce = _config.Observe<float>("jump.force");
            _coyoteTime = _config.Observe<float>("jump.coyote_time");
            _bufferTime = _config.Observe<float>("jump.buffer_time");
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_jumpForce == null) return false;

            if (_actions.Has<JumpAction>())
                _timeSinceJumpRequested = 0f;

            _timeSinceJumpRequested += deltaTime;
            _timeSinceLastGrounded += deltaTime;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                _timeSinceLastGrounded = 0f;
                _jumpConsumed = false;
            }

            var canJump = !_jumpConsumed &&
                          _timeSinceJumpRequested <= _bufferTime.Value &&
                          _timeSinceLastGrounded <= _coyoteTime.Value;

            if (canJump)
            {
                ExecuteJump(motor, ref currentVelocity);
            }

            return false;
        }

        private void ExecuteJump(KinematicCharacterMotor motor, ref Vector3 currentVelocity)
        {
            var jumpDirection = motor.CharacterUp;

            if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
            {
                jumpDirection = motor.GroundingStatus.GroundNormal;
            }

            motor.ForceUnground();

            currentVelocity += jumpDirection * _jumpForce.Value
                - Vector3.Project(currentVelocity, motor.CharacterUp);

            _jumpConsumed = true;
        }
    }
}

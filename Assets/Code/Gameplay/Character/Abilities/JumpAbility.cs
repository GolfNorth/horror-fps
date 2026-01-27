using Game.Gameplay.Character.Configs;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class JumpAbility : MovementAbility
    {
        public override int Priority => 30;

        private CharacterMovementConfig _config;
        private bool _jumpRequested;
        private bool _jumpConsumed;
        private float _timeSinceJumpRequested = float.MaxValue;
        private float _timeSinceLastGrounded = float.MaxValue;

        public bool CanJump => _config != null && !_jumpConsumed && _timeSinceLastGrounded <= _config.CoyoteTime;

        [Inject]
        public void Construct(CharacterMovementConfig config)
        {
            _config = config;
        }

        public void Request()
        {
            _jumpRequested = true;
            _timeSinceJumpRequested = 0f;
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_config == null) return false;

            _timeSinceJumpRequested += deltaTime;
            _timeSinceLastGrounded += deltaTime;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                _timeSinceLastGrounded = 0f;
                _jumpConsumed = false;
            }

            var canJump = !_jumpConsumed &&
                          _timeSinceJumpRequested <= _config.JumpBufferTime &&
                          _timeSinceLastGrounded <= _config.CoyoteTime;

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

            currentVelocity += jumpDirection * _config.JumpForce
                - Vector3.Project(currentVelocity, motor.CharacterUp);

            _jumpRequested = false;
            _jumpConsumed = true;
        }
    }
}

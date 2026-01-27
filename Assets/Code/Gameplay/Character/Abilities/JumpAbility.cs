using KinematicCharacterController;
using UnityEngine;

namespace Game.Gameplay.Character.Abilities
{
    public class JumpAbility : MovementAbility
    {
        public override int Priority => 30;

        [SerializeField] private float _jumpForce = 7f;
        [SerializeField] private float _coyoteTime = 0.15f;
        [SerializeField] private float _jumpBufferTime = 0.1f;

        private bool _jumpRequested;
        private bool _jumpConsumed;
        private float _timeSinceJumpRequested = float.MaxValue;
        private float _timeSinceLastGrounded = float.MaxValue;

        public bool CanJump => !_jumpConsumed && _timeSinceLastGrounded <= _coyoteTime;

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
            _timeSinceJumpRequested += deltaTime;
            _timeSinceLastGrounded += deltaTime;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                _timeSinceLastGrounded = 0f;
                _jumpConsumed = false;
            }

            var canJump = !_jumpConsumed &&
                          _timeSinceJumpRequested <= _jumpBufferTime &&
                          _timeSinceLastGrounded <= _coyoteTime;

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

            currentVelocity += jumpDirection * _jumpForce
                - Vector3.Project(currentVelocity, motor.CharacterUp);

            _jumpRequested = false;
            _jumpConsumed = true;
        }
    }
}

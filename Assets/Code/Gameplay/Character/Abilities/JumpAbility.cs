using Game.Core.Configuration;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class JumpAbility : MovementAbility
    {
        public override int Priority => 30;

        [SerializeField] private string _characterId = "player";

        private IConfigValue<float> _jumpForce;
        private IConfigValue<float> _coyoteTime;
        private IConfigValue<float> _bufferTime;

        private bool _jumpRequested;
        private bool _jumpConsumed;
        private float _timeSinceJumpRequested = float.MaxValue;
        private float _timeSinceLastGrounded = float.MaxValue;

        public bool CanJump => _jumpForce != null && !_jumpConsumed && _timeSinceLastGrounded <= _coyoteTime.Value;

        [Inject]
        public void Construct(IConfigService config)
        {
            var id = _characterId;
            _jumpForce = config.Observe<float>($"{id}.jump.force");
            _coyoteTime = config.Observe<float>($"{id}.jump.coyote_time");
            _bufferTime = config.Observe<float>($"{id}.jump.buffer_time");
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
            if (_jumpForce == null) return false;

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

            _jumpRequested = false;
            _jumpConsumed = true;
        }
    }
}

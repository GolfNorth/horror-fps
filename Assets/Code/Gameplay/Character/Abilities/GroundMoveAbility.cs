using Game.Gameplay.Character.Configs;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class GroundMoveAbility : MovementAbility
    {
        public override int Priority => 10;

        private CharacterMovementConfig _config;
        private Vector3 _moveInput;

        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

        [Inject]
        public void Construct(CharacterMovementConfig config)
        {
            _config = config;
        }

        public void SetMoveInput(Vector3 direction)
        {
            _moveInput = Vector3.ClampMagnitude(direction, 1f);
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_config == null) return false;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                UpdateGroundedVelocity(motor, ref currentVelocity, deltaTime);
            }
            else
            {
                UpdateAirborneVelocity(motor, ref currentVelocity, deltaTime);
            }

            return false;
        }

        private void UpdateGroundedVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (motor.MustUnground())
            {
                UpdateAirborneVelocity(motor, ref currentVelocity, deltaTime);
                return;
            }

            var groundNormal = motor.GroundingStatus.GroundNormal;
            var currentOnGround = Vector3.ProjectOnPlane(currentVelocity, groundNormal);
            var currentSpeed = currentOnGround.magnitude;
            var hasInput = _moveInput.sqrMagnitude > 0.01f;

            if (hasInput)
            {
                var targetDirection = motor.GetDirectionTangentToSurface(_moveInput.normalized, groundNormal);
                var newSpeed = Mathf.MoveTowards(currentSpeed, _config.WalkSpeed, _config.Acceleration * deltaTime);
                currentVelocity = targetDirection * newSpeed;
            }
            else
            {
                var newSpeed = Mathf.MoveTowards(currentSpeed, 0f, _config.Deceleration * deltaTime);
                currentVelocity = currentSpeed > 0.01f
                    ? currentOnGround.normalized * newSpeed
                    : Vector3.zero;
            }
        }

        private void UpdateAirborneVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            var targetHorizontal = _moveInput * (_config.WalkSpeed * _config.AirControl);
            var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

            horizontal = Vector3.MoveTowards(
                horizontal,
                targetHorizontal,
                _config.AirAcceleration * deltaTime);

            currentVelocity = horizontal + Vector3.Project(currentVelocity, motor.CharacterUp);
        }
    }
}

using KinematicCharacterController;
using UnityEngine;

namespace Game.Gameplay.Character.Abilities
{
    public class GroundMoveAbility : MovementAbility
    {
        public override int Priority => 10;

        [Header("Speed")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;

        [Header("Acceleration")]
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 10f;
        [SerializeField] private float _airAcceleration = 5f;
        [SerializeField] private float _airControl = 0.3f;

        private Vector3 _moveInput;
        private bool _isSprinting;

        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;
        public bool IsSprinting => _isSprinting && IsMoving;

        public void SetMoveInput(Vector3 direction)
        {
            _moveInput = Vector3.ClampMagnitude(direction, 1f);
        }

        public void SetSprinting(bool sprinting)
        {
            _isSprinting = sprinting;
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
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
            // If ForceUnground was called (e.g., jump), use airborne logic
            if (motor.MustUnground())
            {
                UpdateAirborneVelocity(motor, ref currentVelocity, deltaTime);
                return;
            }

            var groundNormal = motor.GroundingStatus.GroundNormal;

            // Project current velocity onto ground plane to get current speed
            var currentOnGround = Vector3.ProjectOnPlane(currentVelocity, groundNormal);
            var currentSpeed = currentOnGround.magnitude;

            var targetSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;
            var hasInput = _moveInput.sqrMagnitude > 0.01f;

            if (hasInput)
            {
                // Instant direction change (tangent to ground), smooth speed
                var targetDirection = motor.GetDirectionTangentToSurface(_moveInput.normalized, groundNormal);
                var newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, _acceleration * deltaTime);
                currentVelocity = targetDirection * newSpeed;
            }
            else
            {
                // Decelerate while keeping current direction on ground
                var newSpeed = Mathf.MoveTowards(currentSpeed, 0f, _deceleration * deltaTime);
                if (currentSpeed > 0.01f)
                {
                    currentVelocity = currentOnGround.normalized * newSpeed;
                }
                else
                {
                    currentVelocity = Vector3.zero;
                }
            }
        }

        private void UpdateAirborneVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            var targetHorizontal = _moveInput * (_walkSpeed * _airControl);
            var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

            horizontal = Vector3.MoveTowards(
                horizontal,
                targetHorizontal,
                _airAcceleration * deltaTime);

            currentVelocity = horizontal + Vector3.Project(currentVelocity, motor.CharacterUp);
        }
    }
}

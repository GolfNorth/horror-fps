using Game.Core.Configuration;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class GroundMoveAbility : MovementAbility
    {
        public override int Priority => 10;

        [SerializeField] private string _characterId = "player";

        private IConfigValue<float> _walkSpeed;
        private IConfigValue<float> _acceleration;
        private IConfigValue<float> _deceleration;
        private IConfigValue<float> _airAcceleration;
        private IConfigValue<float> _airControl;

        private Vector3 _moveInput;

        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

        [Inject]
        public void Construct(IConfigService config)
        {
            var id = _characterId;
            _walkSpeed = config.Observe<float>($"{id}.movement.walk_speed");
            _acceleration = config.Observe<float>($"{id}.movement.acceleration");
            _deceleration = config.Observe<float>($"{id}.movement.deceleration");
            _airAcceleration = config.Observe<float>($"{id}.movement.air_acceleration");
            _airControl = config.Observe<float>($"{id}.movement.air_control");
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
            if (_walkSpeed == null) return false;

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
                var newSpeed = Mathf.MoveTowards(currentSpeed, _walkSpeed.Value, _acceleration.Value * deltaTime);
                currentVelocity = targetDirection * newSpeed;
            }
            else
            {
                var newSpeed = Mathf.MoveTowards(currentSpeed, 0f, _deceleration.Value * deltaTime);
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
            var targetHorizontal = _moveInput * (_walkSpeed.Value * _airControl.Value);
            var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

            horizontal = Vector3.MoveTowards(
                horizontal,
                targetHorizontal,
                _airAcceleration.Value * deltaTime);

            currentVelocity = horizontal + Vector3.Project(currentVelocity, motor.CharacterUp);
        }
    }
}

using Game.Core.Configuration;
using Game.Core.Ticking;
using Game.Gameplay.Character.Actions;
using KinematicCharacterController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class GroundMoveAbility : MovementAbility, IInitializable
    {
        public override int Priority => 10;

        private IActionBuffer _actions;
        private CharacterState _state;
        private IConfigService _config;
        private IConfigValue<float> _walkSpeed;
        private IConfigValue<float> _acceleration;
        private IConfigValue<float> _deceleration;
        private IConfigValue<float> _airAcceleration;
        private IConfigValue<float> _airControl;

        [Inject]
        public void Construct(IConfigService config, IActionBuffer actions, CharacterState state)
        {
            _config = config;
            _actions = actions;
            _state = state;
        }

        public void Initialize()
        {
            _walkSpeed = _config.Observe<float>("movement.walk_speed");
            _acceleration = _config.Observe<float>("movement.acceleration");
            _deceleration = _config.Observe<float>("movement.deceleration");
            _airAcceleration = _config.Observe<float>("movement.air_acceleration");
            _airControl = _config.Observe<float>("movement.air_control");
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_walkSpeed == null) return false;

            var moveInput = Vector3.zero;
            if (_actions.TryGet<MoveAction>(out var move))
            {
                moveInput = new Vector3(move.Direction.x, 0f, move.Direction.y);
                moveInput = motor.TransientRotation * moveInput;
                moveInput = Vector3.ClampMagnitude(moveInput, 1f);
            }

            _state.IsMoving.Value = moveInput.sqrMagnitude > 0.01f;

            if (motor.GroundingStatus.IsStableOnGround)
            {
                UpdateGroundedVelocity(motor, ref currentVelocity, moveInput, deltaTime);
            }
            else
            {
                UpdateAirborneVelocity(motor, ref currentVelocity, moveInput, deltaTime);
            }

            return false;
        }

        private void UpdateGroundedVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            Vector3 moveInput,
            float deltaTime)
        {
            if (motor.MustUnground())
            {
                UpdateAirborneVelocity(motor, ref currentVelocity, moveInput, deltaTime);
                return;
            }

            var groundNormal = motor.GroundingStatus.GroundNormal;
            var currentOnGround = Vector3.ProjectOnPlane(currentVelocity, groundNormal);
            var currentSpeed = currentOnGround.magnitude;
            var hasInput = moveInput.sqrMagnitude > 0.01f;

            if (hasInput)
            {
                var targetDirection = motor.GetDirectionTangentToSurface(moveInput.normalized, groundNormal);
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
            Vector3 moveInput,
            float deltaTime)
        {
            var targetHorizontal = moveInput * (_walkSpeed.Value * _airControl.Value);
            var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

            horizontal = Vector3.MoveTowards(
                horizontal,
                targetHorizontal,
                _airAcceleration.Value * deltaTime);

            currentVelocity = horizontal + Vector3.Project(currentVelocity, motor.CharacterUp);
        }
    }
}

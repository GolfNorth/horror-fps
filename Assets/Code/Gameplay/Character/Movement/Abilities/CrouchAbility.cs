using Game.Core.Configuration;
using Game.Gameplay.Character.Actions;
using KinematicCharacterController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class CrouchAbility : MovementAbility, IInitializable
    {
        public override int Priority => 3;

        [Header("Optional")]
        [SerializeField] private Transform _meshRoot;

        private IActionBuffer _actions;
        private CharacterState _state;
        private IConfigService _config;
        private IConfigValue<float> _crouchSpeed;
        private IConfigValue<float> _crouchDeceleration;
        private IConfigValue<float> _crouchHeightRatio;

        private readonly Collider[] _probedColliders = new Collider[8];
        private bool _isCrouching;
        private bool _initialized;

        private float _standingHeight;
        private float _radius;

        [Inject]
        public void Construct(IConfigService config, IActionBuffer actions, CharacterState state)
        {
            _config = config;
            _actions = actions;
            _state = state;
        }

        public void Initialize()
        {
            _crouchSpeed = _config.Observe<float>("crouch.speed");
            _crouchDeceleration = _config.Observe<float>("crouch.deceleration");
            _crouchHeightRatio = _config.Observe<float>("crouch.height_ratio");
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_crouchSpeed == null) return false;

            EnsureInitialized(motor);

            var wantsToCrouch = _actions.Has<CrouchAction>();

            if (wantsToCrouch && !_isCrouching)
            {
                Crouch(motor);
            }

            if (_isCrouching && motor.GroundingStatus.IsStableOnGround)
            {
                var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
                var currentSpeed = horizontal.magnitude;

                if (currentSpeed > _crouchSpeed.Value)
                {
                    var targetSpeed = Mathf.MoveTowards(currentSpeed, _crouchSpeed.Value, _crouchDeceleration.Value * deltaTime);
                    var vertical = Vector3.Project(currentVelocity, motor.CharacterUp);
                    currentVelocity = horizontal.normalized * targetSpeed + vertical;
                }
            }

            _state.IsCrouching.Value = _isCrouching;

            return false;
        }

        public override void AfterCharacterUpdate(KinematicCharacterMotor motor, float deltaTime)
        {
            if (_isCrouching && !_actions.Has<CrouchAction>())
            {
                TryUncrouch(motor);
                _state.IsCrouching.Value = _isCrouching;
            }
        }

        private void EnsureInitialized(KinematicCharacterMotor motor)
        {
            if (_initialized) return;

            _standingHeight = motor.Capsule.height;
            _radius = motor.Capsule.radius;
            _initialized = true;
        }

        private void Crouch(KinematicCharacterMotor motor)
        {
            _isCrouching = true;
            var crouchHeight = _standingHeight * _crouchHeightRatio.Value;
            motor.SetCapsuleDimensions(_radius, crouchHeight, crouchHeight * 0.5f);

            if (_meshRoot != null)
            {
                var scale = _meshRoot.localScale;
                scale.y = _crouchHeightRatio.Value;
                _meshRoot.localScale = scale;
            }
        }

        private void TryUncrouch(KinematicCharacterMotor motor)
        {
            motor.SetCapsuleDimensions(_radius, _standingHeight, _standingHeight * 0.5f);

            if (motor.CharacterCollisionsOverlap(
                    motor.TransientPosition,
                    motor.TransientRotation,
                    _probedColliders) > 0)
            {
                var crouchHeight = _standingHeight * _crouchHeightRatio.Value;
                motor.SetCapsuleDimensions(_radius, crouchHeight, crouchHeight * 0.5f);
            }
            else
            {
                _isCrouching = false;

                if (_meshRoot != null)
                {
                    var scale = _meshRoot.localScale;
                    scale.y = 1f;
                    _meshRoot.localScale = scale;
                }
            }
        }
    }
}

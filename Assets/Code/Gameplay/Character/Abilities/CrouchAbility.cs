using Game.Core.Configuration;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class CrouchAbility : MovementAbility
    {
        public override int Priority => 3;

        [SerializeField] private string _characterId = "player";

        [Header("Optional")]
        [SerializeField] private Transform _meshRoot;

        private IConfigValue<float> _crouchSpeed;
        private IConfigValue<float> _crouchDeceleration;
        private IConfigValue<float> _crouchHeightRatio;

        private readonly Collider[] _probedColliders = new Collider[8];
        private bool _wantsToCrouch;
        private bool _isCrouching;
        private bool _initialized;

        private float _standingHeight;
        private float _radius;

        public bool IsCrouching => _isCrouching;

        [Inject]
        public void Construct(IConfigService config)
        {
            var id = _characterId;
            _crouchSpeed = config.Observe<float>($"{id}.crouch.speed");
            _crouchDeceleration = config.Observe<float>($"{id}.crouch.deceleration");
            _crouchHeightRatio = config.Observe<float>($"{id}.crouch.height_ratio");
        }

        public void SetCrouchInput(bool crouch)
        {
            _wantsToCrouch = crouch;
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_crouchSpeed == null) return false;

            EnsureInitialized(motor);

            if (_wantsToCrouch && !_isCrouching)
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

            return false;
        }

        public override void AfterCharacterUpdate(KinematicCharacterMotor motor, float deltaTime)
        {
            if (_isCrouching && !_wantsToCrouch)
            {
                TryUncrouch(motor);
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

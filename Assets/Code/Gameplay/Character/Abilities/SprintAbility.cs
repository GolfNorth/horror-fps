using Game.Core.Configuration;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class SprintAbility : MovementAbility
    {
        public override int Priority => 5;

        [SerializeField] private CharacterIdProvider _idProvider;

        private IConfigValue<float> _sprintSpeed;
        private IConfigValue<float> _sprintAcceleration;

        private bool _wantsToSprint;

        public bool IsSprinting => _wantsToSprint;

        [Inject]
        public void Construct(IConfigService config)
        {
            var id = _idProvider.CharacterId;
            _sprintSpeed = config.Observe<float>($"{id}.sprint.speed");
            _sprintAcceleration = config.Observe<float>($"{id}.sprint.acceleration");
        }

        public void SetSprintInput(bool sprint)
        {
            _wantsToSprint = sprint;
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_sprintSpeed == null) return false;
            if (!_wantsToSprint || !motor.GroundingStatus.IsStableOnGround)
                return false;

            var horizontal = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
            var currentSpeed = horizontal.magnitude;

            if (currentSpeed < 0.01f)
                return false;

            var targetSpeed = Mathf.MoveTowards(currentSpeed, _sprintSpeed.Value, _sprintAcceleration.Value * deltaTime);
            var vertical = Vector3.Project(currentVelocity, motor.CharacterUp);

            currentVelocity = horizontal.normalized * targetSpeed + vertical;

            return false;
        }

        private void Reset()
        {
            _idProvider = GetComponentInParent<CharacterIdProvider>();
        }
    }
}

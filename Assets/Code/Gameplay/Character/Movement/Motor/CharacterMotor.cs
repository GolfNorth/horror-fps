using System.Linq;
using Game.Gameplay.Character.Movement.Abilities;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Movement.Motor
{
    public class CharacterMotor : MonoBehaviour, ICharacterController
    {
        [SerializeField] private KinematicCharacterMotor _motor;
        [SerializeField] private MovementAbility[] _abilities;

        private CharacterState _state;

        public KinematicCharacterMotor Motor => _motor;
        public bool IsGrounded => _motor.GroundingStatus.IsStableOnGround;
        public Vector3 Velocity => _motor.Velocity;
        public Vector3 GroundNormal => _motor.GroundingStatus.GroundNormal;

        [Inject]
        public void Construct(CharacterState state)
        {
            _state = state;
        }

        private void Awake()
        {
            _motor.CharacterController = this;
            SortAbilities();
        }

        private void SortAbilities()
        {
            _abilities = _abilities
                .Where(a => a != null)
                .OrderByDescending(a => a.Priority)
                .ToArray();
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            foreach (var ability in _abilities)
            {
                if (ability.isActiveAndEnabled &&
                    ability.UpdateVelocity(_motor, ref currentVelocity, deltaTime))
                    break;
            }
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            foreach (var ability in _abilities)
            {
                if (ability.isActiveAndEnabled &&
                    ability.UpdateRotation(_motor, ref currentRotation, deltaTime))
                    break;
            }
        }

        public void BeforeCharacterUpdate(float deltaTime) { }

        public void PostGroundingUpdate(float deltaTime) { }

        public void AfterCharacterUpdate(float deltaTime)
        {
            foreach (var ability in _abilities)
            {
                if (ability.isActiveAndEnabled)
                {
                    ability.AfterCharacterUpdate(_motor, deltaTime);
                }
            }

            if (_state != null)
            {
                _state.IsGrounded.Value = _motor.GroundingStatus.IsStableOnGround;
                _state.Velocity.Value = _motor.Velocity;
            }
        }

        public bool IsColliderValidForCollisions(Collider coll) => true;

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport) { }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport) { }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition, Quaternion atCharacterRotation,
            ref HitStabilityReport hitStabilityReport) { }

        public void OnDiscreteCollisionDetected(Collider hitCollider) { }

        private void Reset()
        {
            _motor = GetComponent<KinematicCharacterMotor>();
            _abilities = GetComponentsInChildren<MovementAbility>();
        }
    }
}

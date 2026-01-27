using KinematicCharacterController;
using UnityEngine;

namespace Game.Gameplay.Character.Abilities
{
    public class BodyRotationAbility : MovementAbility
    {
        public override int Priority => 10;

        private float _targetYaw;

        public float TargetYaw => _targetYaw;

        public void SetTargetYaw(float yaw)
        {
            _targetYaw = yaw;
        }

        public override bool UpdateRotation(
            KinematicCharacterMotor motor,
            ref Quaternion currentRotation,
            float deltaTime)
        {
            currentRotation = Quaternion.Euler(0f, _targetYaw, 0f);
            return false;
        }
    }
}

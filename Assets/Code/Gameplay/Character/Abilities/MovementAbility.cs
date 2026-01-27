using KinematicCharacterController;
using UnityEngine;

namespace Game.Gameplay.Character.Abilities
{
    public abstract class MovementAbility : MonoBehaviour
    {
        public abstract int Priority { get; }

        /// <summary>
        /// Process velocity. Return true to take exclusive control.
        /// </summary>
        public virtual bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime) => false;

        /// <summary>
        /// Process rotation. Return true to take exclusive control.
        /// </summary>
        public virtual bool UpdateRotation(
            KinematicCharacterMotor motor,
            ref Quaternion currentRotation,
            float deltaTime) => false;
    }
}

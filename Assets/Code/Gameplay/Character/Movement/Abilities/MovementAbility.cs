using KinematicCharacterController;
using UnityEngine;

namespace Game.Gameplay.Character.Movement.Abilities
{
    /// <summary>
    /// Base class for movement abilities.
    /// </summary>
    public abstract class MovementAbility : MonoBehaviour, IAbility
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

        /// <summary>
        /// Called after character update. Used for state cleanup and deferred actions.
        /// </summary>
        public virtual void AfterCharacterUpdate(KinematicCharacterMotor motor, float deltaTime) { }
    }
}

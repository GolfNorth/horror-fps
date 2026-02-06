using Game.Gameplay.Character.Actions;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class BodyRotationAbility : MovementAbility
    {
        public override int Priority => 10;

        private IActionBuffer _actions;
        private float _targetYaw;

        [Inject]
        public void Construct(IActionBuffer actions)
        {
            _actions = actions;
        }

        public override bool UpdateRotation(
            KinematicCharacterMotor motor,
            ref Quaternion currentRotation,
            float deltaTime)
        {
            if (_actions != null && _actions.TryGet<LookAction>(out var look))
            {
                _targetYaw += look.Delta.x;
            }

            currentRotation = Quaternion.Euler(0f, _targetYaw, 0f);
            return false;
        }
    }
}

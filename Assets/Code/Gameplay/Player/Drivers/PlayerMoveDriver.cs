using Game.Core.Ticking;
using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerMoveDriver : TickableBehaviour, ITickable
    {
        [SerializeField] private GroundMoveAbility _ability;
        [SerializeField] private Transform _cameraTransform;

        private IPlayerInput _input;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            _input = input;
        }

        public void Tick()
        {
            var moveInput = _input.MoveInput;

            if (moveInput.sqrMagnitude < 0.01f)
            {
                _ability.SetMoveInput(Vector3.zero);
                return;
            }

            var direction = CalculateWorldDirection(moveInput);
            _ability.SetMoveInput(direction);
        }

        private Vector3 CalculateWorldDirection(Vector2 input)
        {
            var forward = _cameraTransform.forward;
            var right = _cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }

        private void Reset()
        {
            _ability = GetComponent<GroundMoveAbility>();
            _cameraTransform = Camera.main?.transform;
        }
    }
}

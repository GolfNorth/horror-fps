using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using R3;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerMoveDriver : MonoBehaviour
    {
        [SerializeField] private GroundMoveAbility _ability;
        [SerializeField] private Transform _cameraTransform;

        private IPlayerInput _input;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            _input = input;

            Observable.EveryUpdate().Subscribe(Tick).AddTo(this);
        }

        private void Tick(Unit _)
        {
            if (_input == null) return;

            var moveInput = _input.MoveInput;

            if (moveInput.sqrMagnitude < 0.01f)
            {
                _ability.SetMoveInput(Vector3.zero);
                _ability.SetSprinting(false);
                return;
            }

            var direction = CalculateWorldDirection(moveInput);
            _ability.SetMoveInput(direction);
            _ability.SetSprinting(_input.IsSprinting);
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

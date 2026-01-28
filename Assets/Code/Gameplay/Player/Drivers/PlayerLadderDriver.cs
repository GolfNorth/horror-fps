using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using R3;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerLadderDriver : MonoBehaviour
    {
        [SerializeField] private LadderClimbAbility _ladderAbility;

        private IPlayerInput _input;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            _input = input;

            _input.OnInteract
                .Subscribe(_ => _ladderAbility.RequestAttach())
                .AddTo(this);

            _input.OnJump
                .Subscribe(_ => _ladderAbility.RequestDetach(withJump: true))
                .AddTo(this);
        }

        private void Update()
        {
            if (_input == null || _ladderAbility == null) return;

            if (_ladderAbility.IsClimbing)
            {
                var moveInput = _input.MoveInput;
                _ladderAbility.SetClimbInput(moveInput.y);
            }
        }

        private void Reset()
        {
            _ladderAbility = GetComponent<LadderClimbAbility>();
        }
    }
}

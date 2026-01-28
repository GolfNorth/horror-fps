using Game.Core.Ticking;
using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerLadderDriver : TickableBehaviour, ITickable
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

        public void Tick()
        {
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

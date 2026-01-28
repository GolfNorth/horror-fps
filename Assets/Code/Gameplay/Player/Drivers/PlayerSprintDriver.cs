using Game.Core.Ticking;
using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerSprintDriver : TickableBehaviour, ITickable
    {
        [SerializeField] private SprintAbility _ability;

        private IPlayerInput _input;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            _input = input;
        }

        public void Tick()
        {
            _ability.SetSprintInput(_input.IsSprinting);
        }

        private void Reset()
        {
            _ability = GetComponent<SprintAbility>();
        }
    }
}

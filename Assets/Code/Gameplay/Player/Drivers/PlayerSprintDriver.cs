using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerSprintDriver : MonoBehaviour
    {
        [SerializeField] private SprintAbility _ability;

        private IPlayerInput _input;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            _input = input;
        }

        private void Update()
        {
            if (_input == null) return;

            _ability.SetSprintInput(_input.IsSprinting);
        }

        private void Reset()
        {
            _ability = GetComponent<SprintAbility>();
        }
    }
}

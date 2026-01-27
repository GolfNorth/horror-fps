using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using R3;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerJumpDriver : MonoBehaviour
    {
        [SerializeField] private JumpAbility _ability;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            input.OnJump
                .Where(_ => enabled)
                .Subscribe(_ => _ability.Request())
                .AddTo(this);
        }

        private void Reset()
        {
            _ability = GetComponent<JumpAbility>();
        }
    }
}

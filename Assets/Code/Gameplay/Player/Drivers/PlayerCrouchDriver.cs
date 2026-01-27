using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using R3;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerCrouchDriver : MonoBehaviour
    {
        [SerializeField] private CrouchAbility _ability;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            input.IsCrouchingChanged
                .Where(_ => enabled)
                .Subscribe(crouching => _ability.SetCrouchInput(crouching))
                .AddTo(this);
        }

        private void Reset()
        {
            _ability = GetComponent<CrouchAbility>();
        }
    }
}

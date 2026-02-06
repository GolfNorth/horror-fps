using Game.Core;
using Game.Core.Conditions;

namespace Game.Gameplay.Character.Conditions
{
    public class IsCrouchingCondition : ICondition
    {
        private readonly CharacterState _state;

        public IsCrouchingCondition(CharacterState state)
        {
            _state = state;
        }

        public bool IsSatisfied() => _state.IsCrouching.Value;

#if UNITY_EDITOR
        public string DisplayName => "Is Crouching";
#endif
    }
}

using Game.Core;
using Game.Core.Conditions;

namespace Game.Gameplay.Character.Conditions
{
    public class IsSprintingCondition : ICondition
    {
        private readonly CharacterState _state;

        public IsSprintingCondition(CharacterState state)
        {
            _state = state;
        }

        public bool IsSatisfied() => _state.IsSprinting.Value;

#if UNITY_EDITOR
        public string DisplayName => "Is Sprinting";
#endif
    }
}

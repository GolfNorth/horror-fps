using Game.Core;
using Game.Core.Conditions;

namespace Game.Gameplay.Character.Conditions
{
    public class IsMovingCondition : ICondition
    {
        private readonly CharacterState _state;

        public IsMovingCondition(CharacterState state)
        {
            _state = state;
        }

        public bool IsSatisfied() => _state.IsMoving.Value;

#if UNITY_EDITOR
        public string DisplayName => "Is Moving";
#endif
    }
}

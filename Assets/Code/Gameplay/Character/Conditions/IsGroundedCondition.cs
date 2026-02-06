using Game.Core;
using Game.Core.Conditions;

namespace Game.Gameplay.Character.Conditions
{
    public class IsGroundedCondition : ICondition
    {
        private readonly CharacterState _state;

        public IsGroundedCondition(CharacterState state)
        {
            _state = state;
        }

        public bool IsSatisfied() => _state.IsGrounded.Value;

#if UNITY_EDITOR
        public string DisplayName => "Is Grounded";
#endif
    }
}

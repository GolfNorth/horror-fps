using System;
using Game.Core.Conditions;
using VContainer;

namespace Game.Gameplay.Character.Conditions
{
    [Serializable]
    public class IsMovingCondition : ICondition
    {
        [NonSerialized] private CharacterState _state;

        public void Bind(IObjectResolver resolver)
        {
            _state = resolver.Resolve<CharacterState>();
        }

        public bool IsSatisfied() => _state != null && _state.IsMoving.Value;

#if UNITY_EDITOR
        public string Label => "Is Moving";
#endif
    }
}

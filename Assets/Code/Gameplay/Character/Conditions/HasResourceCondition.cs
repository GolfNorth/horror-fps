using Game.Core;
using Game.Core.Conditions;
using Game.Gameplay.Character.Resources;

namespace Game.Gameplay.Character.Conditions
{
    public class HasResourceCondition : ICondition
    {
        private readonly SimpleResource _resource;
        private readonly float _minAmount;

        public HasResourceCondition(SimpleResource resource, float minAmount)
        {
            _resource = resource;
            _minAmount = minAmount;
        }

        public bool IsSatisfied() => _resource.CanConsume(_minAmount);

#if UNITY_EDITOR
        public string DisplayName => $"Has Resource (>= {_minAmount})";
#endif
    }
}

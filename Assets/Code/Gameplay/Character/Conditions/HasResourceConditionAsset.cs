using Game.Core.Conditions;
using Game.Gameplay.Character.Resources;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Conditions
{
    [CreateAssetMenu(fileName = "HasResourceCondition", menuName = "Game/Conditions/Character/Has Resource")]
    public class HasResourceConditionAsset : ConditionAsset
    {
        [SerializeField] private float _minAmount = 1f;

        public override ICondition Build(IObjectResolver resolver)
        {
            return new HasResourceCondition(resolver.Resolve<SimpleResource>(), _minAmount);
        }
    }
}

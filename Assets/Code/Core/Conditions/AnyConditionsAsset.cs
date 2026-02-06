using System.Linq;
using UnityEngine;
using VContainer;

namespace Game.Core.Conditions
{
    [CreateAssetMenu(fileName = "AnyConditions", menuName = "Game/Conditions/Any (OR)")]
    public class AnyConditionsAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] _conditions;

        public override ICondition Build(IObjectResolver resolver)
        {
            var built = _conditions
                .Where(c => c != null)
                .Select(c => c.Build(resolver))
                .ToArray();

            return new AnyCondition(built);
        }
    }
}

using System.Linq;
using UnityEngine;
using VContainer;

namespace Game.Core.Conditions
{
    [CreateAssetMenu(fileName = "AllConditions", menuName = "Game/Conditions/All (AND)")]
    public class AllConditionsAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] _conditions;

        public override ICondition Build(IObjectResolver resolver)
        {
            var built = _conditions
                .Where(c => c != null)
                .Select(c => c.Build(resolver))
                .ToArray();

            return new AllCondition(built);
        }
    }
}

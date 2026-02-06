using UnityEngine;
using VContainer;

namespace Game.Core.Conditions
{
    [CreateAssetMenu(fileName = "NotCondition", menuName = "Game/Conditions/Not")]
    public class NotConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset _condition;

        public override ICondition Build(IObjectResolver resolver)
        {
            return new NotCondition(_condition.Build(resolver));
        }
    }
}

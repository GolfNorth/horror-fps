using Game.Core.Conditions;
using UnityEngine;

namespace Game.Gameplay.Character.Rules
{
    [CreateAssetMenu(fileName = "LookActionRule", menuName = "Game/Actions/Rules/Look")]
    public class LookActionRuleAsset : ActionRuleAsset
    {
        protected override IActionRule CreateRule(ICondition[] conditions)
        {
            return new LookActionRule(conditions);
        }
    }
}

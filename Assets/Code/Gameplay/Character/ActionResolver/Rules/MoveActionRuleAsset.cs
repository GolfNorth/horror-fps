using Game.Core.Conditions;
using UnityEngine;

namespace Game.Gameplay.Character.Rules
{
    [CreateAssetMenu(fileName = "MoveActionRule", menuName = "Game/Actions/Rules/Move")]
    public class MoveActionRuleAsset : ActionRuleAsset
    {
        protected override IActionRule CreateRule(ICondition[] conditions)
        {
            return new MoveActionRule(conditions);
        }
    }
}

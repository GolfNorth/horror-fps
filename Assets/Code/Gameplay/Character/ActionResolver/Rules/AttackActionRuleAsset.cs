using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Character.Intents;
using UnityEngine;

namespace Game.Gameplay.Character.Rules
{
    [CreateAssetMenu(fileName = "AttackActionRule", menuName = "Game/Actions/Rules/Attack")]
    public class AttackActionRuleAsset : ActionRuleAsset
    {
        protected override IActionRule CreateRule(ICondition[] conditions)
        {
            return new SimpleActionRule<AttackIntent, AttackAction>(conditions);
        }
    }
}

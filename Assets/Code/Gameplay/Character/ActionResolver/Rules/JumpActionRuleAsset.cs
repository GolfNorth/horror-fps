using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Character.Intents;
using UnityEngine;

namespace Game.Gameplay.Character.Rules
{
    [CreateAssetMenu(fileName = "JumpActionRule", menuName = "Game/Actions/Rules/Jump")]
    public class JumpActionRuleAsset : ActionRuleAsset
    {
        protected override IActionRule CreateRule(ICondition[] conditions)
        {
            return new SimpleActionRule<JumpIntent, JumpAction>(conditions);
        }
    }
}

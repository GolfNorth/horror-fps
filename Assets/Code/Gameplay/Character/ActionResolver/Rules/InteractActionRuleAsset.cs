using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Character.Intents;
using UnityEngine;

namespace Game.Gameplay.Character.Rules
{
    [CreateAssetMenu(fileName = "InteractActionRule", menuName = "Game/Actions/Rules/Interact")]
    public class InteractActionRuleAsset : ActionRuleAsset
    {
        protected override IActionRule CreateRule(ICondition[] conditions)
        {
            return new SimpleActionRule<InteractIntent, InteractAction>(conditions);
        }
    }
}

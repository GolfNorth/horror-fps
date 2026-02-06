using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Player.Input.Intents;
using UnityEngine;

namespace Game.Gameplay.Character.Rules
{
    [CreateAssetMenu(fileName = "SprintActionRule", menuName = "Game/Actions/Rules/Sprint")]
    public class SprintActionRuleAsset : ActionRuleAsset
    {
        protected override IActionRule CreateRule(ICondition[] conditions)
        {
            return new SimpleActionRule<SprintIntent, SprintAction>(conditions);
        }
    }
}

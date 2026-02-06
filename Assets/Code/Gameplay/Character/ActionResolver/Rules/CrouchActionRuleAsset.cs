using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Character.Intents;
using UnityEngine;

namespace Game.Gameplay.Character.Rules
{
    [CreateAssetMenu(fileName = "CrouchActionRule", menuName = "Game/Actions/Rules/Crouch")]
    public class CrouchActionRuleAsset : ActionRuleAsset
    {
        protected override IActionRule CreateRule(ICondition[] conditions)
        {
            return new SimpleActionRule<CrouchIntent, CrouchAction>(conditions);
        }
    }
}

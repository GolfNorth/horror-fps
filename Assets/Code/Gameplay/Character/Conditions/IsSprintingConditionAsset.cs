using Game.Core.Conditions;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Conditions
{
    [CreateAssetMenu(fileName = "IsSprintingCondition", menuName = "Game/Conditions/Character/Is Sprinting")]
    public class IsSprintingConditionAsset : ConditionAsset
    {
        public override ICondition Build(IObjectResolver resolver)
        {
            return new IsSprintingCondition(resolver.Resolve<CharacterState>());
        }
    }
}

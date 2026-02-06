using Game.Core.Conditions;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Conditions
{
    [CreateAssetMenu(fileName = "IsGroundedCondition", menuName = "Game/Conditions/Character/Is Grounded")]
    public class IsGroundedConditionAsset : ConditionAsset
    {
        public override ICondition Build(IObjectResolver resolver)
        {
            return new IsGroundedCondition(resolver.Resolve<CharacterState>());
        }
    }
}

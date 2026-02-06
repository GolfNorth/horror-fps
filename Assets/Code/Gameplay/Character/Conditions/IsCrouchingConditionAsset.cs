using Game.Core.Conditions;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Conditions
{
    [CreateAssetMenu(fileName = "IsCrouchingCondition", menuName = "Game/Conditions/Character/Is Crouching")]
    public class IsCrouchingConditionAsset : ConditionAsset
    {
        public override ICondition Build(IObjectResolver resolver)
        {
            return new IsCrouchingCondition(resolver.Resolve<CharacterState>());
        }
    }
}

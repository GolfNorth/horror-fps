using Game.Core.Conditions;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Conditions
{
    [CreateAssetMenu(fileName = "IsMovingCondition", menuName = "Game/Conditions/Character/Is Moving")]
    public class IsMovingConditionAsset : ConditionAsset
    {
        public override ICondition Build(IObjectResolver resolver)
        {
            return new IsMovingCondition(resolver.Resolve<CharacterState>());
        }
    }
}

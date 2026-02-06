using UnityEngine;
using VContainer;

namespace Game.Core.Conditions
{
    public abstract class ConditionAsset : ScriptableObject
    {
        public abstract ICondition Build(IObjectResolver resolver);
    }
}

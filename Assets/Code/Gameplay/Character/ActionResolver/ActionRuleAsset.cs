using System;
using System.Linq;
using Game.Core.Conditions;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character
{
    public abstract class ActionRuleAsset : ScriptableObject
    {
        [SerializeField] private ConditionAsset[] _conditions;

        public IActionRule Build(IObjectResolver resolver)
        {
            var conditions = _conditions == null || _conditions.Length == 0
                ? Array.Empty<ICondition>()
                : _conditions
                    .Where(c => c != null)
                    .Select(c => c.Build(resolver))
                    .ToArray();

            return CreateRule(conditions);
        }

        protected abstract IActionRule CreateRule(ICondition[] conditions);
    }
}

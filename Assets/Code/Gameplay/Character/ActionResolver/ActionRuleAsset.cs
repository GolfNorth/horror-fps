using System;
using Game.Core.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character
{
    public abstract class ActionRuleAsset : ScriptableObject
    {
        [ListDrawerSettings(ListElementLabelName = "Label"), HideReferenceObjectPicker]
        [SerializeReference] private ICondition[] _conditions;

        public IActionRule Build(IObjectResolver resolver)
        {
            var conditions = _conditions ?? Array.Empty<ICondition>();

            foreach (var condition in conditions)
                condition?.Bind(resolver);

            return CreateRule(conditions);
        }

        protected abstract IActionRule CreateRule(ICondition[] conditions);
    }
}

using System;
using UnityEngine;
using VContainer;

namespace Game.Core.Conditions
{
    [Serializable]
    public class NotCondition : ICondition
    {
        [SerializeReference] private ICondition _condition;

        public void Bind(IObjectResolver resolver)
        {
            _condition?.Bind(resolver);
        }

        public bool IsSatisfied() => _condition != null && !_condition.IsSatisfied();

#if UNITY_EDITOR
        public string Label => _condition != null
            ? $"Not ({_condition.Label})"
            : "Not (empty)";
#endif
    }
}

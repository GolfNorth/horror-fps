using System;
using UnityEngine;
using VContainer;

namespace Game.Core.Conditions
{
    [Serializable]
    public class AllCondition : ICondition
    {
        [SerializeReference] private ICondition[] _conditions;

        public void Bind(IObjectResolver resolver)
        {
            if (_conditions == null) return;
            foreach (var condition in _conditions)
                condition?.Bind(resolver);
        }

        public bool IsSatisfied()
        {
            if (_conditions == null) return true;
            foreach (var condition in _conditions)
            {
                if (condition != null && !condition.IsSatisfied())
                    return false;
            }
            return true;
        }

#if UNITY_EDITOR
        public string Label => "All";
#endif
    }
}

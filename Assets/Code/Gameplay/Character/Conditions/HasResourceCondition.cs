using System;
using Game.Core.Conditions;
using Game.Gameplay.Character.Resources;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Conditions
{
    [Serializable]
    public class HasResourceCondition : ICondition
    {
        [SerializeField] private float _minAmount = 1f;

        [NonSerialized] private SimpleResource _resource;

        public void Bind(IObjectResolver resolver)
        {
            _resource = resolver.Resolve<SimpleResource>();
        }

        public bool IsSatisfied() => _resource != null && _resource.CanConsume(_minAmount);

#if UNITY_EDITOR
        public string DisplayName => $"Has Resource (>= {_minAmount})";
#endif
    }
}

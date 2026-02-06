using System;
using System.Linq;
using Game.Core.Conditions;
using Game.Core.Modules;
using Game.Gameplay.Character.Actions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character
{
    [CreateAssetMenu(fileName = "ActionResolverModule", menuName = "Game/Actions/Action Resolver Module")]
    public class ActionResolverModule : ServicesModule, IActionRuleBuilder
    {
        [SerializeField] private ActionRuleAsset[] _rules;

        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<ActionBuffer>(Lifetime.Scoped).As<IActionBuffer>();
            builder.RegisterInstance<IActionRuleBuilder>(this);
            builder.Register<ActionResolver>(Lifetime.Scoped).As<ITickable>();
        }

        public IActionRule[] BuildRules(IObjectResolver resolver)
        {
            if (_rules == null || _rules.Length == 0)
                return Array.Empty<IActionRule>();

            return _rules
                .Where(r => r != null)
                .Select(r => r.Build(resolver))
                .ToArray();
        }
    }
}

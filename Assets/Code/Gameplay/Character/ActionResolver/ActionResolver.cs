using Game.Gameplay.Character.Actions;
using Game.Gameplay.Player.Input;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character
{
    public class ActionResolver : ITickable
    {
        private readonly IIntentBuffer _intents;
        private readonly IActionBuffer _actions;
        private readonly IActionRule[] _rules;

        public ActionResolver(
            IIntentBuffer intents,
            IActionBuffer actions,
            IActionRuleBuilder ruleBuilder,
            IObjectResolver resolver)
        {
            _intents = intents;
            _actions = actions;
            _rules = ruleBuilder.BuildRules(resolver);
        }

        public void Tick()
        {
            _actions.Clear();

            foreach (var rule in _rules)
                rule.Resolve(_intents, _actions);
        }
    }
}

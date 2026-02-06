using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Player.Input;
using Game.Gameplay.Player.Input.Intents;

namespace Game.Gameplay.Character.Rules
{
    public class SimpleActionRule<TIntent, TAction> : ActionRule
        where TIntent : struct, IIntent
        where TAction : struct, IAction
    {
        public SimpleActionRule(ICondition[] conditions) : base(conditions)
        {
        }

        public override void Resolve(IIntentBuffer intents, IActionBuffer actions)
        {
            if (intents.Has<TIntent>() && CheckConditions())
                actions.Set(default(TAction));
        }
    }
}

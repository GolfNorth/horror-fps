using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Player.Input;
using Game.Gameplay.Player.Input.Intents;

namespace Game.Gameplay.Character.Rules
{
    public class LookActionRule : ActionRule
    {
        public LookActionRule(ICondition[] conditions) : base(conditions)
        {
        }

        public override void Resolve(IIntentBuffer intents, IActionBuffer actions)
        {
            if (intents.TryGet<LookIntent>(out var intent) && CheckConditions())
                actions.Set(new LookAction(intent.Delta));
        }
    }
}

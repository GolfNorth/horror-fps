using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Player.Input;
using Game.Gameplay.Player.Input.Intents;

namespace Game.Gameplay.Character.Rules
{
    public class MoveActionRule : ActionRule
    {
        public MoveActionRule(ICondition[] conditions) : base(conditions)
        {
        }

        public override void Resolve(IIntentBuffer intents, IActionBuffer actions)
        {
            if (intents.TryGet<MoveIntent>(out var intent) && CheckConditions())
                actions.Set(new MoveAction(intent.Direction));
        }
    }
}

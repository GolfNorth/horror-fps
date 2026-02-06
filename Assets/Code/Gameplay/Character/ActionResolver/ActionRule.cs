using Game.Core.Conditions;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Character.Intents;

namespace Game.Gameplay.Character
{
    public abstract class ActionRule : IActionRule
    {
        private readonly ICondition[] _conditions;

        protected ActionRule(ICondition[] conditions)
        {
            _conditions = conditions;
        }

        protected bool CheckConditions()
        {
            foreach (var condition in _conditions)
            {
                if (condition != null && !condition.IsSatisfied())
                    return false;
            }

            return true;
        }

        public abstract void Resolve(IIntentBuffer intents, IActionBuffer actions);
    }
}

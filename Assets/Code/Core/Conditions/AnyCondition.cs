namespace Game.Core.Conditions
{
    public class AnyCondition : ICondition
    {
        private readonly ICondition[] _conditions;

        public AnyCondition(ICondition[] conditions)
        {
            _conditions = conditions;
        }

        public bool IsSatisfied()
        {
            foreach (var condition in _conditions)
            {
                if (condition.IsSatisfied())
                    return true;
            }

            return false;
        }

#if UNITY_EDITOR
        public string DisplayName => "Any";
#endif
    }
}

namespace Game.Core.Conditions
{
    public class AllCondition : ICondition
    {
        private readonly ICondition[] _conditions;

        public AllCondition(ICondition[] conditions)
        {
            _conditions = conditions;
        }

        public bool IsSatisfied()
        {
            foreach (var condition in _conditions)
            {
                if (!condition.IsSatisfied())
                    return false;
            }

            return true;
        }

#if UNITY_EDITOR
        public string DisplayName => "All";
#endif
    }
}

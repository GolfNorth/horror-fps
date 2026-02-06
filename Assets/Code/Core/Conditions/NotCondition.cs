namespace Game.Core.Conditions
{
    public class NotCondition : ICondition
    {
        private readonly ICondition _condition;

        public NotCondition(ICondition condition)
        {
            _condition = condition;
        }

        public bool IsSatisfied() => !_condition.IsSatisfied();

#if UNITY_EDITOR
        public string DisplayName => $"Not ({_condition.DisplayName})";
#endif
    }
}

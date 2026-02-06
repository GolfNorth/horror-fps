namespace Game.Core.Conditions
{
    public interface ICondition : IDisplayName
    {
        bool IsSatisfied();
    }
}

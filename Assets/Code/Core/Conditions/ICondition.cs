using VContainer;

namespace Game.Core.Conditions
{
    public interface ICondition : IDisplayName
    {
        void Bind(IObjectResolver resolver);
        bool IsSatisfied();
    }
}

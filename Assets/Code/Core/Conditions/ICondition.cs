using VContainer;

namespace Game.Core.Conditions
{
    public interface ICondition
    {
        void Bind(IObjectResolver resolver);
        bool IsSatisfied();
#if UNITY_EDITOR
        string Label { get; }
#endif
    }
}

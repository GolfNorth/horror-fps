using VContainer;

namespace Game.Gameplay.Character
{
    public interface IActionRuleBuilder
    {
        IActionRule[] BuildRules(IObjectResolver resolver);
    }
}

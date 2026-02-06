using Game.Gameplay.Character.Actions;
using Game.Gameplay.Character.Intents;

namespace Game.Gameplay.Character
{
    public interface IActionRule
    {
        void Resolve(IIntentBuffer intents, IActionBuffer actions);
    }
}

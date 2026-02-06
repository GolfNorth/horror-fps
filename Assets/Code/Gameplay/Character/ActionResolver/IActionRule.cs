using Game.Gameplay.Character.Actions;
using Game.Gameplay.Player.Input;

namespace Game.Gameplay.Character
{
    public interface IActionRule
    {
        void Resolve(IIntentBuffer intents, IActionBuffer actions);
    }
}

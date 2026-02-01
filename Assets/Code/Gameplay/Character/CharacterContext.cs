using Game.Gameplay.Character.Movement;
using Game.Gameplay.Character.Resources;

namespace Game.Gameplay.Character
{
    /// <summary>
    /// Shared context for character systems.
    /// Contains resources and movement data.
    /// </summary>
    public class CharacterContext
    {
        public SimpleResource Stamina { get; set; }
        public MovementContext Movement { get; } = new();
    }
}

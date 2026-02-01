using Game.Core.DI;

namespace Game.Gameplay
{
    /// <summary>
    /// Scene-level scope for gameplay scenes.
    /// Child of ApplicationScope, contains scene-specific services.
    /// </summary>
    public sealed class GameplayScope : ModularScope
    {
    }
}

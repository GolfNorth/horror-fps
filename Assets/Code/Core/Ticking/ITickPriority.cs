namespace Game.Core.Ticking
{
    /// <summary>
    /// Defines execution priority for tick systems.
    /// Lower values execute first.
    /// </summary>
    public interface ITickPriority
    {
        int TickPriority { get; }
    }
}

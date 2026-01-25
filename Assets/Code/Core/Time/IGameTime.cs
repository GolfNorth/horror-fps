namespace Game.Core.Time
{
    /// <summary>
    /// Abstraction for time access, allowing for pausing and time scaling.
    /// </summary>
    public interface IGameTime
    {
        /// <summary>
        /// Scaled delta time. Returns 0 when paused.
        /// </summary>
        float DeltaTime { get; }

        /// <summary>
        /// Scaled fixed delta time. Returns 0 when paused.
        /// </summary>
        float FixedDeltaTime { get; }

        /// <summary>
        /// Unscaled delta time. Not affected by pause or time scale.
        /// </summary>
        float UnscaledDeltaTime { get; }

        /// <summary>
        /// Current time scale multiplier.
        /// </summary>
        float TimeScale { get; set; }

        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Pause the game.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume the game from pause.
        /// </summary>
        void Resume();
    }
}

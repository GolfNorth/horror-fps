namespace Game.Core.Events
{
    /// <summary>
    /// Game state changed event.
    /// </summary>
    public readonly struct GameStateChangedEvent
    {
        public GameState PreviousState { get; }
        public GameState NewState { get; }

        public GameStateChangedEvent(GameState previous, GameState newState)
        {
            PreviousState = previous;
            NewState = newState;
        }
    }

    /// <summary>
    /// Pause state changed event.
    /// </summary>
    public readonly struct PauseStateChangedEvent
    {
        public bool IsPaused { get; }

        public PauseStateChangedEvent(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }

    /// <summary>
    /// Game states.
    /// </summary>
    public enum GameState
    {
        None,
        Loading,
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}

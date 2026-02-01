namespace Game.Gameplay.Player.Input
{
    /// <summary>
    /// Player input control.
    /// Actual intents are read from IIntentBuffer.
    /// </summary>
    public interface IPlayerInput
    {
        /// <summary>
        /// Whether input is currently enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Enable gameplay input.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable gameplay input (for UI, cutscenes, etc).
        /// </summary>
        void Disable();
    }
}

namespace Game.Gameplay.Player.Input.Intents
{
    /// <summary>
    /// Intent to sprint.
    /// </summary>
    public readonly struct SprintIntent : IIntent
    {
        public override int GetHashCode() => typeof(SprintIntent).GetHashCode();
        public override bool Equals(object obj) => obj is SprintIntent;
    }
}

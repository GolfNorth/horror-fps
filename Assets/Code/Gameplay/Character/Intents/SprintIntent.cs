namespace Game.Gameplay.Character.Intents
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

namespace Game.Gameplay.Character.Intents
{
    /// <summary>
    /// Intent to interact with objects.
    /// </summary>
    public readonly struct InteractIntent : IIntent
    {
        public override int GetHashCode() => typeof(InteractIntent).GetHashCode();
        public override bool Equals(object obj) => obj is InteractIntent;
    }
}

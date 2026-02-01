namespace Game.Gameplay.Player.Input.Intents
{
    /// <summary>
    /// Intent to jump.
    /// </summary>
    public readonly struct JumpIntent : IIntent
    {
        public override int GetHashCode() => typeof(JumpIntent).GetHashCode();
        public override bool Equals(object obj) => obj is JumpIntent;
    }
}

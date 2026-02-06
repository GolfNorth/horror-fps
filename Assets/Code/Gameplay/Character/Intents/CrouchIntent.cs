namespace Game.Gameplay.Character.Intents
{
    /// <summary>
    /// Intent to crouch.
    /// </summary>
    public readonly struct CrouchIntent : IIntent
    {
        public override int GetHashCode() => typeof(CrouchIntent).GetHashCode();
        public override bool Equals(object obj) => obj is CrouchIntent;
    }
}

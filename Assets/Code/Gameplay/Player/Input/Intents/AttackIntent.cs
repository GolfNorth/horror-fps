namespace Game.Gameplay.Player.Input.Intents
{
    /// <summary>
    /// Intent to attack.
    /// </summary>
    public readonly struct AttackIntent : IIntent
    {
        public override int GetHashCode() => typeof(AttackIntent).GetHashCode();
        public override bool Equals(object obj) => obj is AttackIntent;
    }
}

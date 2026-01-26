namespace Game.Player.Abilities
{
    public interface IPlayerAbility
    {
        PlayerAbilityType Type { get; }
        void Initialize();
        void Tick(float deltaTime);
        void FixedTick(float fixedDeltaTime);
        void OnBlocked();
        void OnUnblocked();
    }
}

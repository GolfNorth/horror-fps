using Unity.Mathematics;

namespace Game.Core.Events
{
    /// <summary>
    /// Player spawned event.
    /// </summary>
    public readonly struct PlayerSpawnedEvent
    {
        public int PlayerId { get; }
        public float3 Position { get; }

        public PlayerSpawnedEvent(int playerId, float3 position)
        {
            PlayerId = playerId;
            Position = position;
        }
    }

    /// <summary>
    /// Player death event.
    /// </summary>
    public readonly struct PlayerDeathEvent
    {
        public int PlayerId { get; }
        public DeathCause Cause { get; }

        public PlayerDeathEvent(int playerId, DeathCause cause)
        {
            PlayerId = playerId;
            Cause = cause;
        }
    }

    /// <summary>
    /// Player health changed event.
    /// </summary>
    public readonly struct PlayerHealthChangedEvent
    {
        public int PlayerId { get; }
        public float CurrentHealth { get; }
        public float MaxHealth { get; }
        public float Delta { get; }

        public PlayerHealthChangedEvent(int playerId, float current, float max, float delta)
        {
            PlayerId = playerId;
            CurrentHealth = current;
            MaxHealth = max;
            Delta = delta;
        }
    }

    /// <summary>
    /// Death causes.
    /// </summary>
    public enum DeathCause
    {
        Unknown,
        EnemyDamage,
        FallDamage,
        EnvironmentalHazard,
        Suffocation
    }
}

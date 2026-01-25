using Unity.Mathematics;

namespace Game.Core.Events
{
    /// <summary>
    /// Damage dealt event.
    /// </summary>
    public readonly struct DamageDealtEvent
    {
        public int SourceId { get; }
        public int TargetId { get; }
        public float Damage { get; }
        public DamageType Type { get; }
        public float3 HitPoint { get; }
        public float3 HitNormal { get; }

        public DamageDealtEvent(
            int sourceId,
            int targetId,
            float damage,
            DamageType type,
            float3 hitPoint,
            float3 hitNormal)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Damage = damage;
            Type = type;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
    }

    /// <summary>
    /// Weapon fired event.
    /// </summary>
    public readonly struct WeaponFiredEvent
    {
        public int OwnerId { get; }
        public string WeaponId { get; }
        public float3 MuzzlePosition { get; }
        public float3 Direction { get; }

        public WeaponFiredEvent(int ownerId, string weaponId, float3 muzzle, float3 direction)
        {
            OwnerId = ownerId;
            WeaponId = weaponId;
            MuzzlePosition = muzzle;
            Direction = direction;
        }
    }

    /// <summary>
    /// Weapon reloaded event.
    /// </summary>
    public readonly struct WeaponReloadedEvent
    {
        public int OwnerId { get; }
        public string WeaponId { get; }
        public int CurrentAmmo { get; }
        public int MaxAmmo { get; }

        public WeaponReloadedEvent(int ownerId, string weaponId, int current, int max)
        {
            OwnerId = ownerId;
            WeaponId = weaponId;
            CurrentAmmo = current;
            MaxAmmo = max;
        }
    }

    /// <summary>
    /// Damage types.
    /// </summary>
    public enum DamageType
    {
        Physical,
        Energy,
        Fire,
        Frost,
        Corrosive
    }
}

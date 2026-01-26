using UnityEngine;

namespace Game.Player.Abilities
{
    public abstract class PlayerAbilityBase : MonoBehaviour, IPlayerAbility
    {
        public abstract PlayerAbilityType Type { get; }

        public virtual void Initialize() { }
        public virtual void Tick(float deltaTime) { }
        public virtual void FixedTick(float fixedDeltaTime) { }
        public virtual void OnBlocked() { }
        public virtual void OnUnblocked() { }
    }
}

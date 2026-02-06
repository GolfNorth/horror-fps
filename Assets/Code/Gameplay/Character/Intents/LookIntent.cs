using UnityEngine;

namespace Game.Gameplay.Character.Intents
{
    /// <summary>
    /// Intent to look/rotate camera.
    /// </summary>
    public readonly struct LookIntent : IIntent
    {
        public Vector2 Delta { get; }

        public LookIntent(Vector2 delta)
        {
            Delta = delta;
        }

        public override int GetHashCode() => typeof(LookIntent).GetHashCode();
        public override bool Equals(object obj) => obj is LookIntent;
    }
}

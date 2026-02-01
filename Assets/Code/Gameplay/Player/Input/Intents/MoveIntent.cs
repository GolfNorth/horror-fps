using UnityEngine;

namespace Game.Gameplay.Player.Input.Intents
{
    /// <summary>
    /// Intent to move in a direction.
    /// </summary>
    public readonly struct MoveIntent : IIntent
    {
        public Vector2 Direction { get; }

        public MoveIntent(Vector2 direction)
        {
            Direction = direction;
        }

        public override int GetHashCode() => typeof(MoveIntent).GetHashCode();
        public override bool Equals(object obj) => obj is MoveIntent;
    }
}

using UnityEngine;

namespace Game.Gameplay.Character.Actions
{
    public readonly struct MoveAction : IAction
    {
        public Vector2 Direction { get; }

        public MoveAction(Vector2 direction)
        {
            Direction = direction;
        }
    }
}

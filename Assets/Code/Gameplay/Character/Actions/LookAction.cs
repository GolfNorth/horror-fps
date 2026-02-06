using UnityEngine;

namespace Game.Gameplay.Character.Actions
{
    public readonly struct LookAction : IAction
    {
        public Vector2 Delta { get; }

        public LookAction(Vector2 delta)
        {
            Delta = delta;
        }
    }
}

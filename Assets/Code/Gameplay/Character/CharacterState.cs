using System;
using R3;
using UnityEngine;

namespace Game.Gameplay.Character
{
    public class CharacterState : IDisposable
    {
        public ReactiveProperty<bool> IsGrounded { get; } = new();
        public ReactiveProperty<bool> IsMoving { get; } = new();
        public ReactiveProperty<bool> IsCrouching { get; } = new();
        public ReactiveProperty<bool> IsSprinting { get; } = new();
        public ReactiveProperty<Vector3> Velocity { get; } = new();

        public void Dispose()
        {
            IsGrounded.Dispose();
            IsMoving.Dispose();
            IsCrouching.Dispose();
            IsSprinting.Dispose();
            Velocity.Dispose();
        }
    }
}

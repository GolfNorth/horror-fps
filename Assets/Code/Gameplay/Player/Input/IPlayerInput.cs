using R3;
using UnityEngine;

namespace Game.Gameplay.Player.Input
{
    /// <summary>
    /// Input abstraction. Gameplay systems query intent, not raw input.
    /// </summary>
    public interface IPlayerInput
    {
        /// <summary>
        /// Movement input (WASD/stick).
        /// </summary>
        Vector2 MoveInput { get; }

        /// <summary>
        /// Look input (mouse/stick).
        /// </summary>
        Vector2 LookInput { get; }

        /// <summary>
        /// Whether sprint is being held.
        /// </summary>
        bool IsSprinting { get; }

        /// <summary>
        /// Whether crouch is being held.
        /// </summary>
        bool IsCrouching { get; }

        /// <summary>
        /// Crouch state changed (true = started, false = released).
        /// </summary>
        Observable<bool> IsCrouchingChanged { get; }

        /// <summary>
        /// Jump action triggered.
        /// </summary>
        Observable<Unit> OnJump { get; }

        /// <summary>
        /// Attack action triggered.
        /// </summary>
        Observable<Unit> OnAttack { get; }

        /// <summary>
        /// Interact action triggered.
        /// </summary>
        Observable<Unit> OnInteract { get; }

        /// <summary>
        /// Weapon switch action. Value is direction (-1 previous, +1 next).
        /// </summary>
        Observable<int> OnWeaponSwitch { get; }

        /// <summary>
        /// Enable gameplay input.
        /// </summary>
        void EnableGameplayInput();

        /// <summary>
        /// Disable gameplay input (for UI, cutscenes, etc).
        /// </summary>
        void DisableGameplayInput();
    }
}

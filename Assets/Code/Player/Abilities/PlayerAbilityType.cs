using System;

namespace Game.Player.Abilities
{
    [Flags]
    public enum PlayerAbilityType
    {
        None = 0,
        Movement = 1 << 0,
        Look = 1 << 1,
        Jump = 1 << 2,
        Sprint = 1 << 3,
        Shoot = 1 << 4,
        Reload = 1 << 5,
        Interact = 1 << 6,
        Inventory = 1 << 7,

        AllMovement = Movement | Jump | Sprint,
        AllCombat = Shoot | Reload,
        All = ~0
    }
}

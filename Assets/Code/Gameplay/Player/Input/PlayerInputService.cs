using System;
using Game.Core.Logging;
using Game.Input;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Input
{
    /// <summary>
    /// Wraps Unity Input System and exposes player intent as observable streams.
    /// </summary>
    public sealed class PlayerInputService : IPlayerInput, IInitializable, IDisposable
    {
        private const string LogTag = "PlayerInput";

        private readonly ILogService _log;
        private readonly CompositeDisposable _disposables = new();
        private InputSystem_Actions _inputActions;

        private readonly Subject<Unit> _jumpSubject = new();
        private readonly Subject<Unit> _attackSubject = new();
        private readonly Subject<Unit> _interactSubject = new();
        private readonly Subject<int> _weaponSwitchSubject = new();
        private readonly Subject<bool> _crouchSubject = new();

        public Vector2 MoveInput => _inputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;
        public Vector2 LookInput => _inputActions?.Player.Look.ReadValue<Vector2>() ?? Vector2.zero;
        public bool IsSprinting => _inputActions?.Player.Sprint.IsPressed() ?? false;
        public bool IsCrouching => _inputActions?.Player.Crouch.IsPressed() ?? false;

        public Observable<Unit> OnJump => _jumpSubject;
        public Observable<Unit> OnAttack => _attackSubject;
        public Observable<Unit> OnInteract => _interactSubject;
        public Observable<int> OnWeaponSwitch => _weaponSwitchSubject;
        public Observable<bool> IsCrouchingChanged => _crouchSubject;

        [Inject]
        public PlayerInputService(ILogService log)
        {
            _log = log;
        }

        public void Initialize()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();

            _inputActions.Player.Jump.performed += _ => _jumpSubject.OnNext(Unit.Default);
            _inputActions.Player.Attack.performed += _ => _attackSubject.OnNext(Unit.Default);
            _inputActions.Player.Interact.performed += _ => _interactSubject.OnNext(Unit.Default);
            _inputActions.Player.Previous.performed += _ => _weaponSwitchSubject.OnNext(-1);
            _inputActions.Player.Next.performed += _ => _weaponSwitchSubject.OnNext(1);
            _inputActions.Player.Crouch.started += _ => _crouchSubject.OnNext(true);
            _inputActions.Player.Crouch.canceled += _ => _crouchSubject.OnNext(false);

            _log.Info(LogTag, "Initialized");
        }

        public void EnableGameplayInput()
        {
            _inputActions?.Player.Enable();
        }

        public void DisableGameplayInput()
        {
            _inputActions?.Player.Disable();
        }

        public void Dispose()
        {
            _jumpSubject.Dispose();
            _attackSubject.Dispose();
            _interactSubject.Dispose();
            _weaponSwitchSubject.Dispose();
            _crouchSubject.Dispose();
            _disposables.Dispose();

            _inputActions?.Disable();
            _inputActions?.Dispose();
        }
    }
}

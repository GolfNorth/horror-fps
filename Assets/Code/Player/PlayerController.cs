using System.Collections.Generic;
using Game.Player.Abilities;
using Game.Player.Configs;
using Game.Player.Motor;
using UnityEngine;
using VContainer;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private PlayerAbilityBase[] _abilities;

        private readonly Dictionary<object, PlayerAbilityType> _blockers = new();
        private PlayerAbilityType _blockedAbilities;
        private PlayerMovementConfig _movementConfig;

        public PlayerMotor Motor => _motor;
        public IReadOnlyList<PlayerAbilityBase> Abilities => _abilities;

        #region Blocking API

        public void Block(object blocker, PlayerAbilityType types)
        {
            if (blocker == null || types == PlayerAbilityType.None) return;

            _blockers.TryGetValue(blocker, out var existing);
            _blockers[blocker] = existing | types;

            UpdateBlockedState();
        }

        public void Unblock(object blocker, PlayerAbilityType types)
        {
            if (blocker == null || !_blockers.TryGetValue(blocker, out var existing)) return;

            var remaining = existing & ~types;

            if (remaining == PlayerAbilityType.None)
            {
                _blockers.Remove(blocker);
            }
            else
            {
                _blockers[blocker] = remaining;
            }

            UpdateBlockedState();
        }

        public void UnblockAll(object blocker)
        {
            if (blocker == null) return;

            if (_blockers.Remove(blocker))
            {
                UpdateBlockedState();
            }
        }

        public bool IsBlocked(PlayerAbilityType type)
        {
            return (_blockedAbilities & type) != 0;
        }

        public bool IsAnyBlocked(PlayerAbilityType types)
        {
            return (_blockedAbilities & types) != 0;
        }

        public bool IsBlockedBy(object blocker, PlayerAbilityType type)
        {
            return blocker != null
                && _blockers.TryGetValue(blocker, out var blocked)
                && (blocked & type) != 0;
        }

        public PlayerAbilityType GetBlockedBy(object blocker)
        {
            return blocker != null && _blockers.TryGetValue(blocker, out var blocked)
                ? blocked
                : PlayerAbilityType.None;
        }

        private void UpdateBlockedState()
        {
            var previousBlocked = _blockedAbilities;
            _blockedAbilities = PlayerAbilityType.None;

            foreach (var blocked in _blockers.Values)
            {
                _blockedAbilities |= blocked;
            }

            NotifyBlockedStateChanged(previousBlocked, _blockedAbilities);
        }

        private void NotifyBlockedStateChanged(PlayerAbilityType previous, PlayerAbilityType current)
        {
            foreach (var ability in _abilities)
            {
                if (ability == null) continue;

                var wasBlocked = (previous & ability.Type) != 0;
                var isBlocked = (current & ability.Type) != 0;

                if (!wasBlocked && isBlocked)
                {
                    ability.OnBlocked();
                }
                else if (wasBlocked && !isBlocked)
                {
                    ability.OnUnblocked();
                }
            }
        }

        #endregion

        #region Ability Access

        public T GetAbility<T>() where T : class, IPlayerAbility
        {
            foreach (var ability in _abilities)
            {
                if (ability is T typed)
                {
                    return typed;
                }
            }
            return null;
        }

        public bool TryGetAbility<T>(out T ability) where T : class, IPlayerAbility
        {
            ability = GetAbility<T>();
            return ability != null;
        }

        #endregion

        [Inject]
        public void Construct(PlayerMovementConfig movementConfig)
        {
            _movementConfig = movementConfig;
        }

        private void Start()
        {
            InitializeMotor();
            InitializeAbilities();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            foreach (var ability in _abilities)
            {
                if (ability != null && !IsAnyBlocked(ability.Type))
                {
                    ability.Tick(deltaTime);
                }
            }
        }

        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;

            foreach (var ability in _abilities)
            {
                if (ability != null && !IsAnyBlocked(ability.Type))
                {
                    ability.FixedTick(fixedDeltaTime);
                }
            }
        }

        private void InitializeMotor()
        {
            if (_motor == null)
            {
                _motor = GetComponent<PlayerMotor>();
            }

            _motor.Initialize(_movementConfig);
        }

        private void InitializeAbilities()
        {
            foreach (var ability in _abilities)
            {
                if (ability != null)
                {
                    ability.Initialize();
                }
            }
        }

        private void Reset()
        {
            _motor = GetComponent<PlayerMotor>();
            _abilities = GetComponents<PlayerAbilityBase>();
        }
    }
}

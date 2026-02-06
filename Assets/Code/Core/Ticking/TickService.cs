using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Game.Core.Ticking
{
    public class TickService : MonoBehaviour, ITickService
    {
        private readonly List<TickEntry<ITickable>> _tickables = new();
        private readonly List<TickEntry<ILateTickable>> _lateTickables = new();
        private readonly List<TickEntry<IFixedTickable>> _fixedTickables = new();
        private readonly HashSet<object> _initialized = new();

        private bool _tickablesDirty;
        private bool _lateTickablesDirty;
        private bool _fixedTickablesDirty;

        public void Register(object target)
        {
            if (target == null) return;

            var priority = target is ITickPriority p ? p.TickPriority : 0;
            var hasTickLoop = false;

            if (target is ITickable tickable)
            {
                _tickables.Add(new TickEntry<ITickable>(tickable, priority));
                _tickablesDirty = true;
                hasTickLoop = true;
            }

            if (target is ILateTickable lateTickable)
            {
                _lateTickables.Add(new TickEntry<ILateTickable>(lateTickable, priority));
                _lateTickablesDirty = true;
                hasTickLoop = true;
            }

            if (target is IFixedTickable fixedTickable)
            {
                _fixedTickables.Add(new TickEntry<IFixedTickable>(fixedTickable, priority));
                _fixedTickablesDirty = true;
                hasTickLoop = true;
            }

            if (!hasTickLoop && target is IInitializable initializable)
            {
                initializable.Initialize();
                _initialized.Add(target);
            }
        }

        public void Unregister(object target)
        {
            if (target == null) return;

            _initialized.Remove(target);

            if (target is ITickable tickable)
            {
                RemoveEntry(_tickables, tickable);
                _tickablesDirty = true;
            }

            if (target is ILateTickable lateTickable)
            {
                RemoveEntry(_lateTickables, lateTickable);
                _lateTickablesDirty = true;
            }

            if (target is IFixedTickable fixedTickable)
            {
                RemoveEntry(_fixedTickables, fixedTickable);
                _fixedTickablesDirty = true;
            }
        }

        private void Update()
        {
            if (_tickablesDirty)
            {
                _tickables.Sort(TickEntryComparer<ITickable>.Instance);
                _tickablesDirty = false;
            }

            for (var i = 0; i < _tickables.Count; i++)
            {
                var entry = _tickables[i];
                TryInitialize(entry.Target);
                entry.Target.Tick();
            }
        }

        private void LateUpdate()
        {
            if (_lateTickablesDirty)
            {
                _lateTickables.Sort(TickEntryComparer<ILateTickable>.Instance);
                _lateTickablesDirty = false;
            }

            for (var i = 0; i < _lateTickables.Count; i++)
            {
                var entry = _lateTickables[i];
                TryInitialize(entry.Target);
                entry.Target.LateTick();
            }
        }

        private void FixedUpdate()
        {
            if (_fixedTickablesDirty)
            {
                _fixedTickables.Sort(TickEntryComparer<IFixedTickable>.Instance);
                _fixedTickablesDirty = false;
            }

            for (var i = 0; i < _fixedTickables.Count; i++)
            {
                var entry = _fixedTickables[i];
                TryInitialize(entry.Target);
                entry.Target.FixedTick();
            }
        }

        private void TryInitialize(object target)
        {
            if (_initialized.Contains(target)) return;
            _initialized.Add(target);

            if (target is IInitializable initializable)
            {
                initializable.Initialize();
            }
        }

        private static void RemoveEntry<T>(List<TickEntry<T>> list, T target)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(list[i].Target, target))
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

        private readonly struct TickEntry<T>
        {
            public readonly T Target;
            public readonly int Priority;

            public TickEntry(T target, int priority)
            {
                Target = target;
                Priority = priority;
            }
        }

        private class TickEntryComparer<T> : IComparer<TickEntry<T>>
        {
            public static readonly TickEntryComparer<T> Instance = new();

            public int Compare(TickEntry<T> x, TickEntry<T> y)
            {
                return x.Priority.CompareTo(y.Priority);
            }
        }
    }
}

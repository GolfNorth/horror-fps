using System;
using System.Collections.Generic;

namespace Game.Gameplay.Character.Actions
{
    public sealed class ActionBuffer : IActionBuffer
    {
        private readonly Dictionary<Type, IAction> _actions = new();

        public void Set<T>(T action) where T : struct, IAction
        {
            _actions[typeof(T)] = action;
        }

        public void Remove<T>() where T : struct, IAction
        {
            _actions.Remove(typeof(T));
        }

        public bool Has<T>() where T : struct, IAction
        {
            return _actions.ContainsKey(typeof(T));
        }

        public T Get<T>() where T : struct, IAction
        {
            if (_actions.TryGetValue(typeof(T), out var action))
                return (T)action;

            return default;
        }

        public bool TryGet<T>(out T action) where T : struct, IAction
        {
            if (_actions.TryGetValue(typeof(T), out var stored))
            {
                action = (T)stored;
                return true;
            }

            action = default;
            return false;
        }

        public void Clear()
        {
            _actions.Clear();
        }
    }
}

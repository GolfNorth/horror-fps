using System;
using System.Collections.Generic;
using Game.Gameplay.Player.Input.Intents;

namespace Game.Gameplay.Player.Input
{
    /// <summary>
    /// Buffer implementation using Dictionary for O(1) access by type.
    /// Only one intent per type is stored.
    /// </summary>
    public sealed class IntentBuffer : IIntentBuffer
    {
        private readonly Dictionary<Type, IIntent> _intents = new();

        public void Set<T>(T intent) where T : struct, IIntent
        {
            _intents[typeof(T)] = intent;
        }

        public void Remove<T>() where T : struct, IIntent
        {
            _intents.Remove(typeof(T));
        }

        public bool Has<T>() where T : struct, IIntent
        {
            return _intents.ContainsKey(typeof(T));
        }

        public T Get<T>() where T : struct, IIntent
        {
            if (_intents.TryGetValue(typeof(T), out var intent))
            {
                return (T)intent;
            }
            return default;
        }

        public bool TryGet<T>(out T intent) where T : struct, IIntent
        {
            if (_intents.TryGetValue(typeof(T), out var stored))
            {
                intent = (T)stored;
                return true;
            }
            intent = default;
            return false;
        }

        public IEnumerable<IIntent> GetAll()
        {
            return _intents.Values;
        }

        public void Clear()
        {
            _intents.Clear();
        }
    }
}

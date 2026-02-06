using System;
using UnityEngine;

namespace Game.Gameplay.Character.Resources
{
    /// <summary>
    /// Simple resource component (stamina, health, mana, etc.)
    /// Attach to character GameObject.
    /// </summary>
    public class SimpleResource : MonoBehaviour
    {
        [SerializeField] private float _max = 100f;

        private float _current;

        public event Action<float, float> OnChanged; // current, max

        public float Max => _max;
        public float Current => _current;
        public float Normalized => _max > 0 ? _current / _max : 0f;
        public bool IsEmpty => _current <= 0f;
        public bool IsFull => _current >= _max;

        private void Awake()
        {
            _current = _max;
        }

        public void SetMax(float max, bool fillToMax = false)
        {
            _max = max;
            if (fillToMax || _current > _max)
            {
                _current = _max;
            }
            OnChanged?.Invoke(_current, _max);
        }

        public void Fill()
        {
            if (_current < _max)
            {
                _current = _max;
                OnChanged?.Invoke(_current, _max);
            }
        }

        public void Empty()
        {
            if (_current > 0)
            {
                _current = 0;
                OnChanged?.Invoke(_current, _max);
            }
        }

        public bool CanConsume(float amount)
        {
            return _current >= amount;
        }

        public bool TryConsume(float amount)
        {
            if (_current < amount) return false;

            _current -= amount;
            OnChanged?.Invoke(_current, _max);
            return true;
        }

        public void Consume(float amount)
        {
            var prev = _current;
            _current = Mathf.Max(0f, _current - amount);
            if (!Mathf.Approximately(prev, _current))
            {
                OnChanged?.Invoke(_current, _max);
            }
        }

        public void Restore(float amount)
        {
            var prev = _current;
            _current = Mathf.Min(_max, _current + amount);
            if (!Mathf.Approximately(prev, _current))
            {
                OnChanged?.Invoke(_current, _max);
            }
        }

        public void Set(float value)
        {
            var prev = _current;
            _current = Mathf.Clamp(value, 0f, _max);
            if (!Mathf.Approximately(prev, _current))
            {
                OnChanged?.Invoke(_current, _max);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Game.Core.Configuration
{
    /// <summary>
    /// Config service wrapper that adds scope prefix to lookups.
    /// Resolution order:
    /// 1. Parent with scope prefix ("{scopeId}.{key}")
    /// 2. Parent without prefix ("{key}") - global fallback
    /// Resolved keys are cached for performance.
    /// </summary>
    public class ScopedConfigService : IConfigService
    {
        private readonly IConfigService _parent;
        private readonly string _scopeId;
        private readonly Dictionary<string, string> _resolvedKeys = new();

        public string ScopeId => _scopeId;

        public ScopedConfigService(IConfigService parent, string scopeId)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _scopeId = scopeId ?? throw new ArgumentNullException(nameof(scopeId));
        }

        public IConfigValue<T> Observe<T>(string key) => _parent.Observe<T>(ResolveKey(key));
        public T GetValue<T>(string key) => _parent.GetValue<T>(ResolveKey(key));
        public Type GetValueType(string key) => _parent.GetValueType(ResolveKey(key));
        public void SetValue<T>(string key, T value) => _parent.SetValue(ResolveKey(key), value);
        public void SetValueFromString(string key, string value) => _parent.SetValueFromString(ResolveKey(key), value);
        public void ResetValue(string key) => _parent.ResetValue(ResolveKey(key));
        public void ResetAll() => _parent.ResetAll();

        public bool HasKey(string key)
        {
            return _parent.HasKey(ScopedKey(key)) || _parent.HasKey(key);
        }

        private string ScopedKey(string key) => $"{_scopeId}.{key}";

        private string ResolveKey(string key)
        {
            if (_resolvedKeys.TryGetValue(key, out var cached))
                return cached;

            var scopedKey = ScopedKey(key);
            var resolved = _parent.HasKey(scopedKey) ? scopedKey : key;

            _resolvedKeys[key] = resolved;
            return resolved;
        }
    }
}

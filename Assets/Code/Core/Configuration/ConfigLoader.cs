using UnityEngine;
using VContainer;

namespace Game.Core.Configuration
{
    /// <summary>
    /// Loads ConfigSource assets into ConfigRegistry.
    /// Add to scene and assign config sources.
    /// </summary>
    public class ConfigLoader : MonoBehaviour
    {
        [SerializeField] private ConfigSource[] _sources;

        private ConfigRegistry _registry;

        [Inject]
        public void Construct(ConfigRegistry registry)
        {
            _registry = registry;
            LoadSources();
        }

        private void LoadSources()
        {
            foreach (var source in _sources)
            {
                if (source != null)
                {
                    _registry.RegisterSource(source);
                }
            }
        }

        public void RegisterSource(ConfigSource source)
        {
            _registry.RegisterSource(source);
        }
    }
}

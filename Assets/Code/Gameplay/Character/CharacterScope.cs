using Game.Core.Configuration;
using Game.Core.Modules;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character
{
    /// <summary>
    /// Per-character scope that contains character-specific services.
    /// Attach to character prefab root. Configured via Inspector.
    /// </summary>
    public sealed class CharacterScope : LifetimeScope
    {
        [Header("Identity")]
        [SerializeField] private string _characterId = "player";

        [Header("Configuration")]
        [SerializeField] private ConfigSource[] _configSources;

        [Header("Services")]
        [SerializeField] private ServicesModule[] _serviceModules;

        private CharacterInfo _info;
        private CharacterContext _context;

        public CharacterInfo Info => _info;

        protected override void Awake()
        {
            _info = new CharacterInfo(_characterId);
            _context = new CharacterContext();

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_info);
            builder.RegisterInstance(_context);

            RegisterConfigSources();
            RegisterScopedConfig(builder);
            RegisterServiceModules(builder);
        }

        private void RegisterConfigSources()
        {
            if (_configSources == null || _configSources.Length == 0) return;

            var registry = Parent.Container.Resolve<ConfigRegistry>();
            foreach (var source in _configSources)
            {
                if (source != null)
                {
                    registry.RegisterSource(source);
                }
            }
        }

        private void RegisterScopedConfig(IContainerBuilder builder)
        {
            var parentConfig = Parent.Container.Resolve<IConfigService>();
            var scopedConfig = new ScopedConfigService(parentConfig, _info.Id);
            builder.RegisterInstance(scopedConfig).As<IConfigService>();
        }

        private void RegisterServiceModules(IContainerBuilder builder)
        {
            if (_serviceModules == null) return;

            foreach (var module in _serviceModules)
            {
                module?.Configure(builder);
            }
        }
    }
}

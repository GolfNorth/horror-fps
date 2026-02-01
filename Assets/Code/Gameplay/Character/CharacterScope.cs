using Game.Core.Configuration;
using Game.Core.DI;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character
{
    /// <summary>
    /// Per-character scope that contains character-specific services.
    /// Attach to character prefab root. Configured via Inspector.
    /// </summary>
    public sealed class CharacterScope : ModularScope
    {
        [Header("Identity")]
        [SerializeField] private string _characterId = "player";

        private CharacterInfo _info;
        private CharacterContext _context;

        public CharacterInfo Info => _info;

        protected override void Awake()
        {
            _info = new CharacterInfo(_characterId);
            _context = new CharacterContext();

            base.Awake();
        }

        protected override void ConfigureScope(IContainerBuilder builder)
        {
            builder.RegisterInstance(_info);
            builder.RegisterInstance(_context);

            RegisterScopedConfig(builder);
        }

        private void RegisterScopedConfig(IContainerBuilder builder)
        {
            var parentConfig = Parent.Container.Resolve<IConfigService>();
            var scopedConfig = new ScopedConfigService(parentConfig, _info.Id);
            builder.RegisterInstance(scopedConfig).As<IConfigService>();
        }
    }
}

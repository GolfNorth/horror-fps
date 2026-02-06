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

        protected override void ConfigureScope(IContainerBuilder builder)
        {
            builder.RegisterInstance(new CharacterState());
            RegisterScopedConfig(builder);
        }

        private void RegisterScopedConfig(IContainerBuilder builder)
        {
            var parentConfig = Parent.Container.Resolve<IConfigService>();
            var scopedConfig = new ScopedConfigService(parentConfig, _characterId);
            builder.RegisterInstance(scopedConfig).As<IConfigService>();
        }
    }
}

using Game.Core.Modules;
using UnityEngine;
using VContainer;

namespace Game.Core.Logging
{
    [CreateAssetMenu(
        fileName = "LogServiceModule",
        menuName = "Game/Modules/Log Service")]
    public class LogServiceModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<UnityLogService>(Lifetime.Singleton).As<ILogService>();
        }
    }
}

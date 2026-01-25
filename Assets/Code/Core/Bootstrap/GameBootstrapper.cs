using Game.Core.Logging;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core.Bootstrap
{
    /// <summary>
    /// Application entry point. Runs after DI container is built.
    /// Handles initial application setup.
    /// </summary>
    public sealed class GameBootstrapper : IInitializable
    {
        private const string LogTag = "Game";

        private readonly ILogService _log;

        [Inject]
        public GameBootstrapper(ILogService log)
        {
            _log = log;
        }

        public void Initialize()
        {
            ConfigureApplication();
            ConfigureCursor();

            _log.Info(LogTag, "Bootstrapper initialized");
        }

        private static void ConfigureApplication()
        {
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
        }

        private static void ConfigureCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

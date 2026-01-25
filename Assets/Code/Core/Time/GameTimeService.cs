using Game.Core.Events;
using Game.Core.Logging;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core.Time
{
    /// <summary>
    /// Service managing game time, pause state, and time scaling.
    /// </summary>
    public sealed class GameTimeService : IGameTime, IInitializable, ITickable
    {
        private const string LogTag = "GameTime";

        private readonly IPublisher<PauseStateChangedEvent> _pausePublisher;
        private readonly ILogService _log;

        private float _timeScale = 1f;
        private bool _isPaused;

        public float DeltaTime => _isPaused ? 0f : UnityEngine.Time.deltaTime * _timeScale;
        public float FixedDeltaTime => _isPaused ? 0f : UnityEngine.Time.fixedDeltaTime * _timeScale;
        public float UnscaledDeltaTime => UnityEngine.Time.unscaledDeltaTime;

        public float TimeScale
        {
            get => _timeScale;
            set
            {
                _timeScale = Mathf.Clamp(value, 0f, 10f);
                UnityEngine.Time.timeScale = _isPaused ? 0f : _timeScale;
            }
        }

        public bool IsPaused => _isPaused;

        [Inject]
        public GameTimeService(IPublisher<PauseStateChangedEvent> pausePublisher, ILogService log)
        {
            _pausePublisher = pausePublisher;
            _log = log;
        }

        public void Initialize()
        {
            UnityEngine.Time.timeScale = _timeScale;
            _log.Info(LogTag, "Initialized");
        }

        public void Tick()
        {
        }

        public void Pause()
        {
            if (_isPaused) return;

            _isPaused = true;
            UnityEngine.Time.timeScale = 0f;
            _pausePublisher.Publish(new PauseStateChangedEvent(true));
        }

        public void Resume()
        {
            if (!_isPaused) return;

            _isPaused = false;
            UnityEngine.Time.timeScale = _timeScale;
            _pausePublisher.Publish(new PauseStateChangedEvent(false));
        }
    }
}

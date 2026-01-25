using Game.Core.Configuration;
using UnityEngine;

namespace Game.Core.Logging
{
    [CreateAssetMenu(fileName = "LogConfig", menuName = "Game/Core/Log Config")]
    public sealed class LogConfig : GameConfig
    {
        [SerializeField] private LogLevel _minLevel = LogLevel.Verbose;
        [SerializeField] private bool _colorizeTag = true;

        public LogLevel MinLevel => _minLevel;
        public bool ColorizeTag => _colorizeTag;
    }
}

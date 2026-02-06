using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Core.Logging
{
    [Serializable]
    public struct LogSection : IConfigSection
    {
        public string Key => "log";
        public string Label => "Core/Logging";

        [ConfigKey("min_level")]
        [SerializeField] private LogLevel _minLevel;

        [ConfigKey("colorize_tag")]
        [SerializeField] private bool _colorizeTag;

        public static LogSection Default => new LogSection()
        {
            _minLevel = LogLevel.Verbose,
            _colorizeTag = true
        };
    }
}

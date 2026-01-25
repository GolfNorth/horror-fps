using UnityEngine;
using VContainer;

namespace Game.Core.Logging
{
    /// <summary>
    /// Unity Debug-based logging service implementation.
    /// Tags are colorized based on their hash for easy visual distinction.
    /// </summary>
    public sealed class UnityLogService : ILogService
    {
        private readonly LogConfig _config;

        [Inject]
        public UnityLogService(LogConfig config)
        {
            _config = config;
        }

        public void Info(string tag, string message)
        {
            if (_config.MinLevel > LogLevel.Info)
                return;
            Debug.Log(FormatMessage(tag, message));
        }

        public void Info(string tag, string message, Object context)
        {
            if (_config.MinLevel > LogLevel.Info)
                return;
            Debug.Log(FormatMessage(tag, message), context);
        }

        public void Verbose(string tag, string message)
        {
            if (_config.MinLevel > LogLevel.Verbose)
                return;
            Debug.Log(FormatMessage(tag, message));
        }

        public void Warning(string tag, string message)
        {
            if (_config.MinLevel > LogLevel.Warning)
                return;
            Debug.LogWarning(FormatMessage(tag, message));
        }

        public void Warning(string tag, string message, Object context)
        {
            if (_config.MinLevel > LogLevel.Warning)
                return;
            Debug.LogWarning(FormatMessage(tag, message), context);
        }

        public void Error(string tag, string message)
        {
            if (_config.MinLevel > LogLevel.Error)
                return;
            Debug.LogError(FormatMessage(tag, message));
        }

        public void Error(string tag, string message, Object context)
        {
            if (_config.MinLevel > LogLevel.Error)
                return;
            Debug.LogError(FormatMessage(tag, message), context);
        }

        public void Exception(System.Exception exception)
        {
            Debug.LogException(exception);
        }

        public void Exception(System.Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }

        private string FormatMessage(string tag, string message)
        {
            if (_config.ColorizeTag)
            {
                var color = GetTagColor(tag);
                return $"<color={color}>[{tag}]</color> {message}";
            }
            return $"[{tag}] {message}";
        }

        private static string GetTagColor(string tag)
        {
            var hash = tag.GetHashCode();
            var hue = (hash & 0x7FFFFFFF) % 360;
            return HslToHex(hue, 0.7f, 0.65f);
        }

        private static string HslToHex(int h, float s, float l)
        {
            var c = (1f - Mathf.Abs(2f * l - 1f)) * s;
            var x = c * (1f - Mathf.Abs(h / 60f % 2f - 1f));
            var m = l - c / 2f;

            var (r, g, b) = h switch
            {
                < 60 => (c, x, 0f),
                < 120 => (x, c, 0f),
                < 180 => (0f, c, x),
                < 240 => (0f, x, c),
                < 300 => (x, 0f, c),
                _ => (c, 0f, x)
            };

            var ri = Mathf.RoundToInt((r + m) * 255);
            var gi = Mathf.RoundToInt((g + m) * 255);
            var bi = Mathf.RoundToInt((b + m) * 255);

            return $"#{ri:X2}{gi:X2}{bi:X2}";
        }
    }
}
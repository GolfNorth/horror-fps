using UnityEngine;

namespace Game.Core.Logging
{
    /// <summary>
    /// Logging service interface.
    /// Inject this into services for testable logging.
    /// </summary>
    public interface ILogService
    {
        void Info(string tag, string message);
        void Info(string tag, string message, Object context);
        void Verbose(string tag, string message);
        void Warning(string tag, string message);
        void Warning(string tag, string message, Object context);
        void Error(string tag, string message);
        void Error(string tag, string message, Object context);
        void Exception(System.Exception exception);
        void Exception(System.Exception exception, Object context);
    }
}

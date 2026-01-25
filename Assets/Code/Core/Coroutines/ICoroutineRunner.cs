using System.Collections;
using UnityEngine;

namespace Game.Core.Coroutines
{
    /// <summary>
    /// Abstraction for running coroutines from non-MonoBehaviour classes.
    /// </summary>
    public interface ICoroutineRunner
    {
        /// <summary>
        /// Start a coroutine.
        /// </summary>
        Coroutine Run(IEnumerator coroutine);

        /// <summary>
        /// Stop a running coroutine.
        /// </summary>
        void Stop(Coroutine coroutine);

        /// <summary>
        /// Stop all running coroutines.
        /// </summary>
        void StopAll();
    }
}
